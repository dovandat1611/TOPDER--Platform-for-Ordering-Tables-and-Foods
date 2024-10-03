using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderTableService
    {
        //Task<bool> AddAsync(List<CreateOrUpdateOrderTableDto> orderTablesDto);
        Task<List<OrderTableDto>> GetItemsByOrderAsync(int id);
        Task<bool> AddAsync(CreateRestaurantOrderTablesDto orderTablesDto);

    }
}
