using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;

namespace TOPDER.Repository.Repositories
{
    internal class OrderMenuRepository : GenericRepository<OrderMenu>, IOrderMenuRepository
    {
        public OrderMenuRepository(TopderDBContext dbContext) : base(dbContext) { }
    }
}
