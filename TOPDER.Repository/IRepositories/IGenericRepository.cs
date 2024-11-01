using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Repository.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);
        Task<T> CreateAndReturnAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IQueryable<T>> QueryableAsync();
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteRangeAsync(List<T> entities);
        Task<bool> CreateRangeAsync(List<T> entities);
        Task<bool> ChangeStatusAsync(int id, string status);
    }
}
