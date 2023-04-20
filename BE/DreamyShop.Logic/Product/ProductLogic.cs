﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DreamyShop.Common.Exceptions;
using DreamyShop.Common.Extensions;
using DreamyShop.Common.Results;
using DreamyShop.Domain;
using DreamyShop.Domain.Shared.Dtos;
using DreamyShop.Domain.Shared.Types;
using DreamyShop.EntityFrameworkCore;
using DreamyShop.Logic.Conditions;
using DreamyShop.Repository.RepositoryWrapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DreamyShop.Logic.Product
{
    public class ProductLogic : IProductLogic
    {
        private readonly DreamyShopDbContext _context;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public ProductLogic(
            DreamyShopDbContext context,
            IRepositoryWrapper repository,
            IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
        }

        #region Product
        public async Task<ApiResult<PageResult<ProductDto>>> GetAllProduct(PagingRequest pagingRequest)
        {
            var query = await (from p in _context.Products
                               join m in _context.Manufacturers on p.ManufacturerId equals m.Id
                               join c in _context.ProductCategories on p.CategoryId equals c.Id
                               join pv in _context.ProductVariants on p.Id equals pv.ProductId into pvN
                               from pv in pvN.DefaultIfEmpty()
                               join pvv in _context.ProductVariantValues on pv.Id equals pvv.ProductVariantId into pvvN
                               from pvv in pvvN.DefaultIfEmpty()
                               join pav in _context.ProductAttributeValues on pvv.ProductAttributeValueId equals pav.Id into pavN
                               from pav in pavN.DefaultIfEmpty()
                               select new
                               {
                                   Product = p,
                                   ProductVariantId = pvv.ProductVariantId == null ? Guid.Empty : pvv.ProductVariantId,
                                   ManufacturerName = m.Name,
                                   CategoryName = c.Name,
                                   pv,
                                   pav
                               }).ToListAsync();

            var productPagings = query.GroupBy(r => new { r.Product })
                        .Select(x => new ProductDto
                        {
                            Id = x.Key.Product.Id,
                            Name = x.Key.Product.Name,
                            Code = x.Key.Product.Code,
                            ThumbnailPicture = x.Key.Product.ThumbnailPicture ?? "",
                            ProductType = x.Key.Product?.ProductType ?? ProductType.Single,
                            CategoryName = x.FirstOrDefault()?.CategoryName ?? "",
                            ManufacturerName = x.FirstOrDefault()?.ManufacturerName ?? "",
                            Description = x.Key.Product?.Description ?? "",
                            IsActive = x.Key.Product?.IsActive ?? true,
                            IsVisibility = x.Key.Product?.IsVisibility ?? true,
                            DateCreated = x.Key.Product?.DateCreated ?? DateTime.Now,
                            DateUpdated = x.Key.Product?.DateUpdated ?? DateTime.Now,
                            ProductAttributeDisplayDtos = x.GroupBy(p => p.ProductVariantId)
                                                        .Select(pAttr => new ProductAttributeDisplayDto
                                                        {
                                                            AttributeNames = pAttr.Select(x => x.pav?.Value ?? "").ToList(),
                                                            Quantity = pAttr.Select(x => x.pv?.Quantity ?? 0).FirstOrDefault(),
                                                            Price = pAttr.Select(x => x.pv?.Price ?? 0).FirstOrDefault()
                                                        }).ToList()
                        }).ToList();

            //var productPagings = _context.Products
            //                    .Include(opt => opt.Manufacturer)
            //                    .Include(opt => opt.ProductCategory)
            //                    .Include(opt => opt.ProductVariants)
            //                    .Include(opt => opt.ProductVariantValues)
            //                    .Include(opt => opt.ProductAttributeValues)
            //                    .OrderByDescending(p => p.DateCreated)
            //                    .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            //                    .Skip((pagingRequest.Page - 1) * pagingRequest.Limit)
            //                    .Take(pagingRequest.Limit)
            //                    .ToList();

            var pageResult = new PageResult<ProductDto>()
            {
                Items = productPagings,
                Totals = productPagings.Count()
            };
            return new ApiSuccessResult<PageResult<ProductDto>>(pageResult);
        }

        public async Task<ApiResult<ProductDto>> CreateProduct(ProductCreateUpdateDto productCreateUpdateDto)
        {
            var attributes =  _repository.Attribute.GetAll().ToList();
            AddAttribute(productCreateUpdateDto, attributes);
            AddManufacturer(productCreateUpdateDto.ManufacturerName);

            var newProduct = new Domain.Product
            {
                ManufacturerId = _repository.Manufacturer.GetByName(productCreateUpdateDto.ManufacturerName).Id,
                Name = productCreateUpdateDto.Name,
                Code = productCreateUpdateDto.Code,
                Slug = null,
                SortOrder = 1,
                ProductType = productCreateUpdateDto.ProductType,
                CategoryId = _repository.Category.GetByName(productCreateUpdateDto.CategoryName).Id,
                SeoMetaDescription = null,
                Description = productCreateUpdateDto.Description,
                ThumbnailPicture = productCreateUpdateDto.ThumbnailPicture,
                IsActive = productCreateUpdateDto.IsActive,
                IsVisibility = productCreateUpdateDto.IsVisibility
            };
            await _repository.Product.AddAsync(newProduct);
            _repository.Save();

            AddProductAttributeValue(productCreateUpdateDto, attributes, newProduct);

            return new ApiSuccessResult<ProductDto>(_mapper.Map<ProductDto>(newProduct));
        }

        /// <summary>
        /// Add ProductAttributeValue relate to Product
        /// </summary>
        /// <param name="productCreateUpdateDto"></param>
        /// <param name="attributes"></param>
        /// <param name="newProduct"></param>
        private async void AddProductAttributeValue(ProductCreateUpdateDto productCreateUpdateDto, List<Domain.Attribute> attributes, Domain.Product newProduct)
        {
            var attributeCreates = productCreateUpdateDto.ProductOptions.ToList();
            var productAttributeNames = productCreateUpdateDto.VariantProducts.SelectMany(pad => pad.AttributeNames.Select(a => a.Standard())).Distinct().ToList();
            var productAttributeValues = _repository.ProductAttributeValue.GetAll().ToList();
            var existingProductAttributeValues = productAttributeValues.Where(pa => productAttributeNames.Contains(pa.Value.Standard())).ToList();
            var newProductAttributeNames = productAttributeNames.Where(an => existingProductAttributeValues.All(a => a.Value.Standard() != an.Standard())).ToList();

            var newProductAttributes = newProductAttributeNames.Select(an => new ProductAttributeValue
            {
                AttributeId = attributes.FirstOrDefault(aTrr => aTrr.Name.Standard() == attributeCreates
                                    .Where(a => a.Value.Select(p => p.Standard()).Contains(an))
                                    .FirstOrDefault().Key).Id,
                ProductId = newProduct.Id,
                Value = an
            }).ToList();
            _repository.Save();
        }

        /// <summary>
        /// Add if there are many variant products
        /// </summary>
        /// <param name="productCreateUpdateDto"></param>
        /// <param name="attributes"></param>
        private async void AddAttribute(ProductCreateUpdateDto productCreateUpdateDto, List<Domain.Attribute> attributes)
        {
            if (productCreateUpdateDto.VariantProducts.Any(pAttr => pAttr.AttributeNames != null) && productCreateUpdateDto.VariantProducts.Count > 1)
            {
                var attributeNames = productCreateUpdateDto.ProductOptions.Select(pad => pad.Key.Standard()).Distinct().ToList();
                var existingAttributes = attributes.Where(a => attributeNames.Contains(a.Name.Standard())).ToList();
                var newAttributeNames = attributeNames.Where(an => existingAttributes.All(a => a.Name.Standard() != an.Standard())).ToList();
                var newAttributes = newAttributeNames.Select(an => new Domain.Attribute
                {
                    Name = an,
                    Code = an.ToUpper(),
                    IsActive = true,
                    IsVisibility = true,
                    IsUnique = true,
                    Note = ""
                }).ToList();
                await _repository.Attribute.AddRangeAsync(newAttributes);
                _repository.Save();
            }
        }

        /// <summary>
        /// Add Manufacturer relate to Product
        /// </summary>
        /// <param name="manufacturerName"></param>
        private async void AddManufacturer(string manufacturerName)
        {
            var manufacturers = _repository.Manufacturer.GetAll();
            if (manufacturers.Select(m => m.Name.Standard()).ToList().Any(m => m == manufacturerName.Standard()))
            {
                await _repository.Manufacturer.AddAsync(new Domain.Manufacturer
                {
                    Name = manufacturerName,
                    Code = manufacturerName.ToUpper(),
                    Slug = "",
                    CoverPicture = "",
                    IsVisibility = true,
                    IsActive = true,
                    Country = ""
                });
                _repository.Save();
            }
        }



        //public async Task<ApiResult<ProductDto>> UpdateProduct(Guid id, ProductCreateUpdateDto productCreateUpdateDto)
        //{
        //    var product = await _repository.Product.GetByIdAsync(id);
        //    if (product == null)
        //    {
        //        return new ApiErrorResult<ProductDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //    }
        //    _repository.Product.Update(_mapper.Map(productCreateUpdateDto, product));
        //    _repository.Save();
        //    return new ApiSuccessResult<ProductDto>(_mapper.Map<ProductDto>(product));
        //}
        //public async Task<ApiResult<bool>> RemoveProduct(Guid id)
        //{
        //    var product = await _repository.Product.GetByIdAsync(id);
        //    if (product == null)
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //    try
        //    {
        //        await _repository.Product.BeginTransactionAsync();
        //        _repository.Product.Remove(id);
        //        _repository.Save();
        //        await _repository.Product.EndTransactionAsync();
        //    }
        //    catch
        //    {
        //        await _repository.Product.RollbackTransactionAsync();
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //    }
        //    return new ApiSuccessResult<bool>(true);
        //}
        //public async Task<ApiResult<IList<ProductDto>>> SearchProduct(SearchProductCondition condition)
        //{
        //    var pagingRequest = new PagingRequest() { Page = 1, Limit = 10 };
        //    var products = _context.Products
        //                        .Include(opt => opt.Manufacturer)
        //                        .Include(opt => opt.ProductCategory)
        //                        .OrderByDescending(p => p.DateCreated)
        //                        .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
        //                        .ToList();
        //    if (condition == null)
        //    {
        //        return new ApiSuccessResult<IList<ProductDto>>(GetAllProduct(pagingRequest).Result.Result.Items);
        //    }
        //    if (condition.ProductName != null)
        //    {
        //        products = products.Where(p => p.Name.ToLower() == condition.ProductName.ToLower()).ToList();
        //    }
        //    if (condition.Code != null)
        //    {
        //        products = products.Where(p => p.Code == condition.Code).ToList();
        //    }
        //    if (condition.Price != null)
        //    {
        //        products = products.Where(p => p.Price == condition.Price).ToList();
        //    }
        //    if (condition.ProductType != null)
        //    {
        //        products = products.Where(p => p.ProductType == condition.ProductType).ToList();
        //    }
        //    if (condition.CategoryName != null)
        //    {
        //        products = products.Where(p => p.CategoryName == condition.CategoryName).ToList();
        //    }
        //    if (condition.ManufacturerName != null)
        //    {
        //        products = products.Where(p => p.ManufacturerName == condition.ManufacturerName).ToList();
        //    }
        //    if (condition.IsActive != null)
        //    {
        //        products = products.Where(p => p.IsActive == condition.IsActive).ToList();
        //    }
        //    if (condition.IsVisibility != null)
        //    {
        //        products = products.Where(p => p.IsVisibility == condition.IsVisibility).ToList();
        //    }
        //    if (condition.DateCreated != null)
        //    {
        //        products = products.Where(p => p.DateCreated == condition.DateCreated).ToList();
        //    }
        //    if (condition.DateUpdated != null)
        //    {
        //        products = products.Where(p => p.DateUpdated == condition.DateUpdated).ToList();
        //    }
        //    var productPagingsResult = products.Skip((pagingRequest.Page - 1) * pagingRequest.Limit)
        //                            .Take(pagingRequest.Limit)
        //                            .ToList(); ;
        //    return new ApiSuccessResult<IList<ProductDto>>(_mapper.Map<List<ProductDto>>(productPagingsResult));
        //}
        #endregion

        //#region ProductAttribute
        //public async Task<ApiResult<PageResult<ProductAttributeValueDto>>> GetListProductAttributeValue(Guid productId, PagingRequest pagingRequest)
        //{
        //    var product = await _repository.Product.GetByIdAsync(productId);
        //    var productAttributes = from pa in _context.ProductAttributes
        //                            join padt in _context.ProductAttributeDateTimes on pa.Id equals padt.AttributeId into aDateTimeTable
        //                            from padt in aDateTimeTable.DefaultIfEmpty()
        //                            join pad in _context.ProductAttributeDecimals on pa.Id equals pad.AttributeId into aDecimalTable
        //                            from pad in aDecimalTable.DefaultIfEmpty()
        //                            join pai in _context.ProductAttributeInts on pa.Id equals pai.AttributeId into aIntTable
        //                            from pai in aIntTable.DefaultIfEmpty()
        //                            join pat in _context.ProductAttributeTexts on pa.Id equals pat.AttributeId into aTextTable
        //                            from pat in aTextTable.DefaultIfEmpty()
        //                            join pavc in _context.ProductAttributeVarchars on pa.Id equals pavc.AttributeId into aVarcharTable
        //                            from pavc in aVarcharTable.DefaultIfEmpty()
        //                            where (padt == null || padt.ProductId == productId)
        //                            && (pad == null || pad.ProductId == productId)
        //                            && (pai == null || pai.ProductId == productId)
        //                            && (pat == null || pat.ProductId == productId)
        //                            && (pavc == null || pavc.ProductId == productId)
        //                            select new ProductAttributeValueDto()
        //                            {
        //                                Name = pa.Name,
        //                                AttributeId = pa.Id,
        //                                DataType = pa.DataType,
        //                                Code = pa.Code,
        //                                ProductId = productId,
        //                                DateTimeValue = padt != null ? padt.Value : null,
        //                                DecimalValue = pad != null ? pad.Value : null,
        //                                IntValue = pai != null ? pai.Value : null,
        //                                TextValue = pat != null ? pat.Value : null,
        //                                VarcharValue = pavc != null ? pavc.Value : null,
        //                                DateTimeId = padt != null ? padt.Id : null,
        //                                DecimalId = pad != null ? pad.Id : null,
        //                                IntId = pai != null ? pai.Id : null,
        //                                TextId = pat != null ? pat.Id : null,
        //                                VarcharId = pavc != null ? pavc.Id : null,
        //                            };
        //    productAttributes = productAttributes.Where(x => x.DateTimeId != null
        //                   || x.DecimalId != null
        //                   || x.IntValue != null
        //                   || x.TextId != null
        //                   || x.VarcharId != null);

        //    var pageResult = new PageResult<ProductAttributeValueDto>()
        //    {
        //        Items = productAttributes.ToList() ?? new List<ProductAttributeValueDto>(),
        //        Totals = productAttributes.Count()
        //    };
        //    return new ApiSuccessResult<PageResult<ProductAttributeValueDto>>(pageResult);
        //}

        //public async Task<ApiResult<bool>> CreateAtributeValueProduct(CreateProductAttributeValueDto productAttributeDto)
        //{
        //    var product = await _repository.Product.GetByIdAsync(productAttributeDto.ProductId);
        //    if (product == null)
        //    {
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //    }
        //    var attribute = await _repository.ProductAttribute.GetByIdAsync(productAttributeDto.AttributeId);
        //    if (attribute == null)
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //    var newAttributeId = Guid.NewGuid();
        //    switch (attribute.DataType)
        //    {
        //        case AttributeType.Date:
        //            if (productAttributeDto.DateTimeValue == null)
        //            {
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //            }
        //            var productAttributeDateTime = new ProductAttributeDateTime
        //                                                (newAttributeId,
        //                                                productAttributeDto.AttributeId,
        //                                                productAttributeDto.ProductId,
        //                                                productAttributeDto.DateTimeValue,
        //                                                attribute,
        //                                                product);
        //            await _repository.ProductAttributeDateTime.AddAsync(productAttributeDateTime);
        //            break;

        //        case AttributeType.Int:
        //            if (productAttributeDto.IntValue == null)
        //            {
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //            }
        //            var productAttributeInt = new ProductAttributeInt
        //                                            (newAttributeId,
        //                                            productAttributeDto.AttributeId,
        //                                            productAttributeDto.ProductId,
        //                                            productAttributeDto.IntValue.Value,
        //                                            attribute,
        //                                            product);
        //            await _repository.ProductAttributeInt.AddAsync(productAttributeInt);
        //            break;

        //        case AttributeType.Decimal:
        //            if (productAttributeDto.DecimalValue == null)
        //            {
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //            }
        //            var productAttributeDecimal = new ProductAttributeDecimal
        //                                            (newAttributeId,
        //                                            productAttributeDto.AttributeId,
        //                                            productAttributeDto.ProductId,
        //                                            productAttributeDto.DecimalValue.Value,
        //                                            attribute,
        //                                            product);
        //            await _repository.ProductAttributeDecimal.AddAsync(productAttributeDecimal);
        //            break;

        //        case AttributeType.Varchar:
        //            if (productAttributeDto.VarcharValue == null)
        //            {
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //            }
        //            var productAttributeVarchar = new ProductAttributeVarchar
        //                                            (newAttributeId,
        //                                            productAttributeDto.AttributeId,
        //                                            productAttributeDto.ProductId,
        //                                            productAttributeDto.VarcharValue,
        //                                            attribute,
        //                                            product);
        //            await _repository.ProductAttributeVarchar.AddAsync(productAttributeVarchar);
        //            break;

        //        case AttributeType.Text:
        //            if (productAttributeDto.TextValue == null)
        //            {
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //            }
        //            var productAttributeText = new ProductAttributeText
        //                                            (newAttributeId,
        //                                            productAttributeDto.AttributeId,
        //                                            productAttributeDto.ProductId,
        //                                            productAttributeDto.TextValue,
        //                                            attribute,
        //                                            product);
        //            await _repository.ProductAttributeText.AddAsync(productAttributeText);
        //            break;
        //    }
        //    _repository.Save();
        //    return new ApiSuccessResult<bool>(true);
        //}
        //public async Task<ApiResult<ProductAttributeValueDto>> UpdateProductAttributeValue(Guid attributeId, CreateProductAttributeValueDto updateProductAttributeDto)
        //{
        //    var product = await _repository.Product.GetByIdAsync(updateProductAttributeDto.ProductId);
        //    if (product == null)
        //    {
        //        return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //    }
        //    var attribute = await _repository.ProductAttribute.GetByIdAsync(updateProductAttributeDto.AttributeId);
        //    if (attribute == null)
        //        return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //    switch (attribute.DataType)
        //    {
        //        case AttributeType.Date:
        //            var attributeDateTime = await _repository.ProductAttributeDateTime.GetByIdAsync(attributeId);
        //            if (attributeDateTime == null || updateProductAttributeDto.DateTimeValue == null)
        //                return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //            attributeDateTime.Value = updateProductAttributeDto.DateTimeValue.Value;
        //            _repository.ProductAttributeDateTime.Update(attributeDateTime);
        //            break;
        //        case AttributeType.Int:
        //            var attributeInt = await _repository.ProductAttributeInt.GetByIdAsync(attributeId);
        //            if (attributeInt == null || updateProductAttributeDto.IntValue == null)
        //                return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //            attributeInt.Value = updateProductAttributeDto.IntValue.Value;
        //            _repository.ProductAttributeInt.Update(attributeInt);
        //            break;
        //        case AttributeType.Decimal:
        //            var attributeDecimal = await _repository.ProductAttributeDecimal.GetByIdAsync(attributeId);
        //            if (attributeDecimal == null || updateProductAttributeDto.DecimalValue == null)
        //                return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //            attributeDecimal.Value = updateProductAttributeDto.DecimalValue.Value;
        //            _repository.ProductAttributeDecimal.Update(attributeDecimal);
        //            break;
        //        case AttributeType.Varchar:
        //            var attributeVarchar = await _repository.ProductAttributeVarchar.GetByIdAsync(attributeId);
        //            if (attributeVarchar == null || updateProductAttributeDto.VarcharValue == null)
        //                return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //            attributeVarchar.Value = updateProductAttributeDto.VarcharValue;
        //            _repository.ProductAttributeVarchar.Update(attributeVarchar);
        //            break;
        //        case AttributeType.Text:
        //            var attributeText = await _repository.ProductAttributeText.GetByIdAsync(attributeId);
        //            if (attributeText == null || updateProductAttributeDto.TextValue == null)
        //                return new ApiErrorResult<ProductAttributeValueDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //            attributeText.Value = updateProductAttributeDto.TextValue;
        //            _repository.ProductAttributeText.Update(attributeText);
        //            break;
        //    }
        //    _repository.Save();
        //    return new ApiSuccessResult<ProductAttributeValueDto>(new ProductAttributeValueDto
        //    {
        //        AttributeId = updateProductAttributeDto.AttributeId,
        //        Code = attribute.Code,
        //        DataType = attribute.DataType,
        //        DateTimeValue = updateProductAttributeDto.DateTimeValue,
        //        DecimalValue = updateProductAttributeDto.DecimalValue,
        //        Id = attributeId,
        //        IntValue = updateProductAttributeDto.IntValue,
        //        Name = attribute.Name,
        //        ProductId = updateProductAttributeDto.ProductId,
        //        TextValue = updateProductAttributeDto.TextValue
        //    });
        //}

        //public async Task<ApiResult<bool>> RemoveProductAttributeValue(Guid attributeId, Guid attributeTypeId)
        //{
        //    var attribute = await _repository.ProductAttribute.GetByIdAsync(attributeId);
        //    if (attribute == null)
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //    switch (attribute.DataType)
        //    {
        //        case AttributeType.Date:
        //            try
        //            {
        //                await _repository.ProductAttributeDateTime.BeginTransactionAsync();
        //                _repository.ProductAttributeDateTime.Remove(attributeTypeId);
        //                _repository.Save();
        //                await _repository.ProductAttributeDateTime.EndTransactionAsync();
        //            }
        //            catch
        //            {
        //                await _repository.ProductAttributeDateTime.RollbackTransactionAsync();
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //            }
        //            break;

        //        case AttributeType.Int:
        //            try
        //            {
        //                await _repository.ProductAttributeInt.BeginTransactionAsync();
        //                _repository.ProductAttributeInt.Remove(attributeTypeId);
        //                _repository.Save();
        //                await _repository.ProductAttributeInt.EndTransactionAsync();
        //            }
        //            catch
        //            {
        //                await _repository.ProductAttributeInt.RollbackTransactionAsync();
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //            }
        //            break;

        //        case AttributeType.Decimal:
        //            try
        //            {
        //                await _repository.ProductAttributeDecimal.BeginTransactionAsync();
        //                _repository.ProductAttributeDecimal.Remove(attributeTypeId);
        //                _repository.Save();
        //                await _repository.ProductAttributeDecimal.EndTransactionAsync();
        //            }
        //            catch
        //            {
        //                await _repository.ProductAttributeDecimal.RollbackTransactionAsync();
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //            }
        //            break;

        //        case AttributeType.Varchar:
        //            try
        //            {
        //                await _repository.ProductAttributeVarchar.BeginTransactionAsync();
        //                _repository.ProductAttributeVarchar.Remove(attributeTypeId);
        //                _repository.Save();
        //                await _repository.ProductAttributeVarchar.EndTransactionAsync();
        //            }
        //            catch
        //            {
        //                await _repository.ProductAttributeVarchar.RollbackTransactionAsync();
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //            }
        //            break;

        //        case AttributeType.Text:
        //            try
        //            {
        //                await _repository.ProductAttributeText.BeginTransactionAsync();
        //                _repository.ProductAttributeText.Remove(attributeTypeId);
        //                _repository.Save();
        //                await _repository.ProductAttributeText.EndTransactionAsync();
        //            }
        //            catch
        //            {
        //                await _repository.ProductAttributeText.RollbackTransactionAsync();
        //                return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //            }
        //            break;
        //    }
        //    _repository.Save();
        //    return new ApiSuccessResult<bool>(true);
        //}
        //#endregion

        //#region ProductAttributeValue
        //public async Task<ApiResult<PageResult<ProductAttributeDto>>> GetListProductAttribute(PagingRequest pagingRequest)
        //{
        //    var productAttributes = _repository.ProductAttribute.GetAll()
        //                            .ProjectTo<ProductAttributeDto>(_mapper.ConfigurationProvider)
        //                            .Skip((pagingRequest.Page - 1) * pagingRequest.Limit)
        //                            .Take(pagingRequest.Limit)
        //                            .ToList();
        //    var pageResult = new PageResult<ProductAttributeDto>()
        //    {
        //        Items = productAttributes,
        //        Totals = productAttributes.Count()
        //    };
        //    return new ApiSuccessResult<PageResult<ProductAttributeDto>>(pageResult);
        //}

        //public async Task<ApiResult<ProductAttributeDto>> CreateAtributeProduct(CreateProductAttributeDto productAttributeDto)
        //{
        //    var newProductAttribute = _mapper.Map<Domain.ProductAttribute>(productAttributeDto);
        //    await _repository.ProductAttribute.AddAsync(newProductAttribute);
        //    _repository.Save();
        //    return new ApiSuccessResult<ProductAttributeDto>(_mapper.Map<ProductAttributeDto>(newProductAttribute));
        //}

        //public async Task<ApiResult<ProductAttributeDto>> UpdateProductAttribute(Guid id, CreateProductAttributeDto updateProductAttributeDto)
        //{
        //    var productAttribute = await _repository.ProductAttribute.GetByIdAsync(id);
        //    if (productAttribute == null)
        //    {
        //        return new ApiErrorResult<ProductAttributeDto>((int)ErrorCodes.DataEntryIsNotExisted);
        //    }
        //    _repository.ProductAttribute.Update(_mapper.Map(updateProductAttributeDto, productAttribute));
        //    _repository.Save();
        //    return new ApiSuccessResult<ProductAttributeDto>(_mapper.Map<ProductAttributeDto>(productAttribute));
        //}

        //public async Task<ApiResult<bool>> RemoveProductAttribute(Guid attributeId)
        //{
        //    var productAttribute = await _repository.ProductAttribute.GetByIdAsync(attributeId);
        //    if (productAttribute == null)
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DataEntryIsNotExisted);
        //    try
        //    {
        //        await _repository.ProductAttribute.BeginTransactionAsync();
        //        _repository.ProductAttribute.Remove(attributeId);
        //        _repository.Save();
        //        await _repository.ProductAttribute.EndTransactionAsync();
        //    }
        //    catch
        //    {
        //        await _repository.ProductAttribute.RollbackTransactionAsync();
        //        return new ApiErrorResult<bool>((int)ErrorCodes.DeleteFailed);
        //    }
        //    return new ApiSuccessResult<bool>(true);
        //}
        //#endregion
    }
}