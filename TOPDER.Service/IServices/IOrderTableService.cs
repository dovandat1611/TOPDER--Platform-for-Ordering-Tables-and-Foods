using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderTableService
    {
        Task<bool> AddAsync(List<CreateOrUpdateOrderTableDto> orderTableDtos);
        Task<List<OrderTableDto>> GetItemsByOrderAsync(int id);
    }
}
