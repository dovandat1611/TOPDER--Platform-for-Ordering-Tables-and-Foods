using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;

namespace TOPDER.Repository.IRepositories
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<IEnumerable<Restaurant>> GetAllItemsAsync();
        IQueryable<Restaurant> GetAllItems();
    }
}
