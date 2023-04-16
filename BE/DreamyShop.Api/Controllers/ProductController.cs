﻿using DreamyShop.Api.Authorization;
using DreamyShop.Domain.Shared.Dtos;
using DreamyShop.Logic.Conditions;
using DreamyShop.Logic.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = DreamyShop.Api.Authorization.AuthorizeAttribute;

namespace DreamyShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductLogic _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(
            IProductLogic productService,
            ILogger<ProductController> logger)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProduct([FromQuery] PagingRequest pagingRequest)
        {
            var result = await _productService.GetAllProduct(pagingRequest);
            return Ok(result.Result.Items);
        }

        //[HttpPost("create")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> CreateProduct([FromForm] ProductCreateUpdateDto productCreateUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.CreateProduct(productCreateUpdateDto);
        //    return Ok(result);
        //}

        //[HttpPut("updateProduct")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductCreateUpdateDto productCreateUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.UpdateProduct(id, productCreateUpdateDto);
        //    return Ok(result);
        //}

        //[HttpDelete("removeProduct")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> RemoveProduct(Guid id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.RemoveProduct(id);
        //    return Ok(result);
        //}

        //[HttpGet("getListProductAttributeValue")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> GetListProductAttributeValue(Guid productId, PagingRequest pagingRequest)
        //{
        //    var result = await _productService.GetListProductAttributeValue(productId, pagingRequest);
        //    return Ok(result);
        //}

        //[HttpPost("createAtributeValueProduct")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> CreateAtributeValueProduct([FromForm] CreateProductAttributeValueDto productAttributeDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.CreateAtributeValueProduct(productAttributeDto);
        //    return Ok(result);
        //}

        //[HttpPut("updateProductAttributeValue")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> UpdateProductAttributeValue(Guid id, [FromForm] CreateProductAttributeValueDto productAttributeDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.UpdateProductAttributeValue(id, productAttributeDto);
        //    return Ok(result);
        //}

        //[HttpDelete("removeProductAttributeValue")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> RemoveProductAttributeValue(Guid attributeId, Guid attributeTypeId)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.RemoveProductAttributeValue(attributeId, attributeTypeId);
        //    return Ok(result);
        //}


        //[HttpPut("searchCondition")]
        //public async Task<IActionResult> SearchProduct([FromForm] SearchProductCondition searchProductCondition)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var result = await _productService.SearchProduct(searchProductCondition);
        //    if (result.Result == null)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        //[HttpGet("getAllProductAttribute")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> GetAllProductAttribute(PagingRequest pagingRequest)
        //{
        //    var result = await _productService.GetListProductAttribute(pagingRequest);
        //    return Ok(result);
        //}

        //[HttpPost("createProductAttribute")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> CreateProductAttribute([FromForm] CreateProductAttributeDto productCreateUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.CreateAtributeProduct(productCreateUpdateDto);
        //    return Ok(result);
        //}

        //[HttpPut("updateProductAttribute")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> UpdateProductAttribute(Guid id, [FromForm] CreateProductAttributeDto productCreateUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.UpdateProductAttribute(id, productCreateUpdateDto);
        //    return Ok(result);
        //}

        //[HttpDelete("removeProductAttribute")]
        //[Authorize]
        //[Member]
        //public async Task<IActionResult> RemoveProductAttribute(Guid id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _productService.RemoveProductAttribute(id);
        //    return Ok(result);
        //}
    }
}
