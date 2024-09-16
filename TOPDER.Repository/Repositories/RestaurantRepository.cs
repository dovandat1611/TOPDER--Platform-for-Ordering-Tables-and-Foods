using Microsoft.EntityFrameworkCore;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TOPDER.Repository.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(TopderDBContext dbContext) : base(dbContext) {}
    }
}
