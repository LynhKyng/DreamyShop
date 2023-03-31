﻿using DreamyShop.EntityFrameworkCore;
using DreamyShop.Repository.Repositories.Auth;
using DreamyShop.Repository.Repositories.Product;
using DreamyShop.Repository.Repositories.User;

namespace DreamyShop.Repository.RepositoryWrapper
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly DreamyShopDbContext _context;
        private IAuthRepository _auth;
        private IUserRepository _user;
        private IProductRepository _product;
        public RepositoryWrapper(
            DreamyShopDbContext context, 
            IAuthRepository auth,
            IUserRepository user,
            IProductRepository product)
        {
            _context = context;
            _auth = auth;
            _user = user;
            _product = product;
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