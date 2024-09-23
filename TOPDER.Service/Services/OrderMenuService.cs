using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class OrderMenuService : IOrderMenuService
    {
        public Task<bool> AddAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderMenuDto>> GetItemsByOrderAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
