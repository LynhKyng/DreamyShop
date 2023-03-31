﻿using DreamyShop.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DreamyShop.Repository.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DreamyShopDbContext _context;

        private readonly DbSet<T> _dbSet;

        public GenericRepository(DreamyShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync() => _context.Database.BeginTransactionAsync();

        public Task RollbackTransactionAsync() => _context.Database.RollbackTransactionAsync();

        public async Task EndTransactionAsync() => await _context.Database.CommitTransactionAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            return await _dbSet.FindAsync(id);
        }

        public void Remove(Guid id)
        {
            var result = _dbSet.Find(id);
            try
            {
                _dbSet.Remove(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveMultiple(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}