using Microsoft.EntityFrameworkCore;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TOPDER.Repository.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        private readonly TopderDBContext _context;

        public RestaurantRepository(TopderDBContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Restaurant>> GetAllItemsAsync()
        {
            return await _context.Restaurants
                        .Include(r => r.Reviews)
                        .ToListAsync();
        }

        public IQueryable<Restaurant> GetAllItems()
        {
            return _context.Restaurants
                .Include(r => r.Reviews);
        }

    }
}
