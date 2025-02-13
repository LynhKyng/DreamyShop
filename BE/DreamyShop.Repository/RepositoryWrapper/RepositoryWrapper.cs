﻿using DreamyShop.EntityFrameworkCore;
using DreamyShop.Repository.Repositories.Auth;
using DreamyShop.Repository.Repositories.Bill;
using DreamyShop.Repository.Repositories.Cart;
using DreamyShop.Repository.Repositories.Category;
using DreamyShop.Repository.Repositories.Manufacturer;
using DreamyShop.Repository.Repositories.Product;
using DreamyShop.Repository.Repositories.Role;
using DreamyShop.Repository.Repositories.User;

namespace DreamyShop.Repository.RepositoryWrapper
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly DreamyShopDbContext _context;
        private IAuthRepository _auth;
        private IUserRepository _user;
        private IProductRepository _product;
        private IProductVariantRepository _productVariant;
        private IProductImageRepository _productImage;
        private IProductVariantValueRepository _productVariantValue;
        private IProductAttributeRepository _productAttribute;
        private IAttributeRepository _attribute;
        private IProductAttributeValueRepository _productAttributeValue;
        private IManufacturerRepository _manufacturer;
        private ICategoryRepository _category;
        private ICartRepository _cart;
        private ICartDetailRepository _cartDetail;
        private IBillRepository _bill;
        private IBillDetailRepository _billDetail;
        private IRoleRepository _role;
        public RepositoryWrapper(
            DreamyShopDbContext context, 
            IAuthRepository auth,
            IUserRepository user,
            IRoleRepository role,
            IProductRepository product,
            IProductAttributeRepository productAttribute,
            IProductAttributeValueRepository productAttributeValue,
            IManufacturerRepository manufacturer,
            ICategoryRepository category,
            IAttributeRepository attribute,
            IProductVariantRepository productVariant,
            IProductVariantValueRepository productVariantValue,
            IProductImageRepository productImage,
            ICartRepository cart,
            ICartDetailRepository cartDetail,
            IBillRepository bill,
            IBillDetailRepository billDetail
            )
        {
            _context = context;
            _auth = auth;
            _user = user;
            _product = product;
            _productAttribute = productAttribute;
            _productAttributeValue = productAttributeValue;
            _manufacturer = manufacturer;
            _category = category;
            _role = role;
            _attribute = attribute;
            _productVariant = productVariant;
            _productVariantValue = productVariantValue;
            _productImage = productImage;
            _cart = cart;
            _cartDetail = cartDetail;
            _bill = bill;
            _billDetail = billDetail;
        }

        public IAuthRepository Auth
        {
            get
            {
                if (_auth == null)
                {
                    _auth = new AuthRepository(_context);
                }
                return _auth;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_context);
                }
                return _user;
            }
        }

        public IRoleRepository Role
        {
            get
            {
                if (_role == null)
                {
                    _role = new RoleRepository(_context);
                }
                return _role;
            }
        }

        public IProductRepository Product
        {
            get
            {
                if (_product == null)
                {
                    _product = new ProductRepository(_context);
                }
                return _product;
            }
        }

        public IProductAttributeRepository ProductAttribute
        {
            get
            {
                if (_productAttribute == null)
                {
                    _productAttribute = new ProductAttributeRepository(_context);
                }
                return _productAttribute;
            }
        }

        public IProductAttributeValueRepository ProductAttributeValue
        {
            get
            {
                if (_productAttributeValue == null)
                {
                    _productAttributeValue = new ProductAttributeValueRepository(_context);
                }
                return _productAttributeValue;
            }
        }
        public IManufacturerRepository Manufacturer
        {
            get
            {
                if (_manufacturer == null)
                {
                    _manufacturer = new ManufacturerRepository(_context);
                }
                return _manufacturer;
            }
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_context);
                }
                return _category;
            }
        }

        public IAttributeRepository Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = new AttributeRepository(_context);
                }
                return _attribute;
            }
        }

        public IProductVariantRepository ProductVariant
        {
            get
            {
                if (_productVariant == null)
                {
                    _productVariant = new ProductVariantRepository(_context);
                }
                return _productVariant;
            }
        }

        public IProductVariantValueRepository ProductVariantValue
        {
            get
            {
                if (_productVariantValue == null)
                {
                    _productVariantValue = new ProductVariantValueRepository(_context);
                }
                return _productVariantValue;
            }
        }

        public IProductImageRepository ProductImage
        {
            get
            {
                if (_productImage == null)
                {
                    _productImage = new ProductImageRepository(_context);
                }
                return _productImage;
            }
        }

        public ICartRepository Cart
        {
            get
            {
                if (_cart == null)
                {
                    _cart = new CartRepository(_context);
                }
                return _cart;
            }
        }

        public ICartDetailRepository CartDetail
        {
            get
            {
                if (_cartDetail == null)
                {
                    _cartDetail = new CartDetailRepository(_context);
                }
                return _cartDetail;
            }
        }

        public IBillRepository Bill
        {
            get
            {
                if (_bill == null)
                {
                    _bill = new BillRepository(_context);
                }
                return _bill;
            }
        }

        public IBillDetailRepository BillDetail
        {
            get
            {
                if (_billDetail == null)
                {
                    _billDetail = new BillDetailRepository(_context);
                }
                return _billDetail;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}