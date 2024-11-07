using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderMenuService
    {
        Task<bool> AddRangeAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos);
        Task<bool> ChangeMenusAsync(int orderId, List<CreateOrUpdateOrderMenuDto> orderMenuDtos);
        Task<List<OrderMenuDto>> GetItemsByOrderAsync(int id);
    }
}
