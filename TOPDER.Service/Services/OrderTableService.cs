using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class OrderTableService : IOrderTableService
    {
        public Task<bool> AddAsync(List<CreateOrUpdateOrderTableDto> orderTableDtos)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderTableDto>> GetItemsByOrderAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
