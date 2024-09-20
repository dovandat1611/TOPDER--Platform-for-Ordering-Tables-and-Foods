using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;

namespace TOPDER.Repository.Repositories
{
    public class OrderTableRepository : GenericRepository<OrderTable>, IOrderTableRepository
    {
        public OrderTableRepository(TopderDBContext dbContext) : base(dbContext) { }
    }
}
