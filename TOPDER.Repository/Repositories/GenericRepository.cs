using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // For IDbContextTransaction
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;

namespace TOPDER.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TopderDBContext _dbContext;

        public GenericRepository(TopderDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IQueryable<T>> QueryableAsync()
        {
            return await Task.FromResult(_dbContext.Set<T>().AsQueryable());
        }

        public async Task<bool> CreateAsync(T entity)
        {
            if (entity == null)
                return false;

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Set<T>().AddAsync(entity);
                    int result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit nếu không có lỗi
                    return result > 0;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }

        public async Task<T> CreateAndReturnAsync(T entity)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Set<T>().AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit sau khi thành công
                    return entity;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback nếu có lỗi
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _dbContext.Set<T>().FindAsync(id);
                    if (entity == null)
                        return false;

                    _dbContext.Set<T>().Remove(entity);
                    int result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit sau khi xoá thành công
                    return result > 0;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }


        public async Task<bool> DeleteRangeAsync(List<T> entities)
        {
            if (entities == null || !entities.Any())
                return false;

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Set<T>().RemoveRange(entities); // Xoá danh sách các thực thể
                    int result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit sau khi xoá thành công
                    return result > 0;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} was not found.");
            }
            return entity;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Set<T>().Update(entity);
                    int result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit sau khi cập nhật thành công
                    return result > 0;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }

        public async Task<bool> CreateRangeAsync(List<T> entities)
        {
            if (entities == null || entities.Count == 0)
                return false;

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Set<T>().AddRangeAsync(entities);
                    int result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit nếu thành công
                    return result > 0;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }

        public async Task<bool> ChangeStatusAsync(int id, string status)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await GetByIdAsync(id);
                    if (entity == null)
                    {
                        return false;
                    }

                    var property = typeof(T).GetProperty("Status");
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(entity, status);
                    }

                    await UpdateAsync(entity);

                    await transaction.CommitAsync(); // Commit sau khi cập nhật trạng thái
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback khi có lỗi
                    throw;
                }
            }
        }
    }
}