// EMS.DAL/Repository/Repository.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EMS.DAL.Data;

namespace EMS.DAL.Repository
{
    /// <summary>
    /// Generic repository interface: basic CRUD + query helpers.
    /// All domain-specific repositories extend this contract.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>>  GetAllAsync();
        Task<T?>              GetByIdAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?>             GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task                 AddAsync(T entity);
        Task                 AddRangeAsync(IEnumerable<T> entities);
        Task                 UpdateAsync(T entity);
        Task                 RemoveAsync(T entity);
        Task                 RemoveRangeAsync(IEnumerable<T> entities);
        Task<bool>           AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int>            CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task                 SaveChangesAsync();
    }

    /// <summary>
    /// EF Core-backed generic repository.  Concrete repositories inherit this
    /// class and inject <see cref="EMSContext"/> through their constructors.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EMSContext  _context;
        protected readonly DbSet<T>    _dbSet;

        public Repository(EMSContext context)
        {
            _context = context;
            _dbSet   = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try { return await _dbSet.ToListAsync(); }
            catch (Exception ex) { throw new Exception($"Error retrieving all {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            try { return await _dbSet.FindAsync(id); }
            catch (Exception ex) { throw new Exception($"Error retrieving {typeof(T).Name} by ID: {ex.Message}", ex); }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try { return await _dbSet.Where(predicate).ToListAsync(); }
            catch (Exception ex) { throw new Exception($"Error finding {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            try { return await _dbSet.FirstOrDefaultAsync(predicate); }
            catch (Exception ex) { throw new Exception($"Error retrieving single {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task AddAsync(T entity)
        {
            try { await _dbSet.AddAsync(entity); await SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error adding {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            try { await _dbSet.AddRangeAsync(entities); await SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error adding multiple {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task UpdateAsync(T entity)
        {
            try { _dbSet.Update(entity); await SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error updating {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task RemoveAsync(T entity)
        {
            try { _dbSet.Remove(entity); await SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error removing {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            try { _dbSet.RemoveRange(entities); await SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error removing multiple {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try { return await _dbSet.AnyAsync(predicate); }
            catch (Exception ex) { throw new Exception($"Error checking {typeof(T).Name} existence: {ex.Message}", ex); }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                return predicate == null
                    ? await _dbSet.CountAsync()
                    : await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex) { throw new Exception($"Error counting {typeof(T).Name}: {ex.Message}", ex); }
        }

        public async Task SaveChangesAsync()
        {
            try { await _context.SaveChangesAsync(); }
            catch (Exception ex) { throw new Exception($"Error saving changes: {ex.Message}", ex); }
        }
    }
}