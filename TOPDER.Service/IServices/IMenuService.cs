using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IMenuService
    {
        Task<bool> AddAsync(MenuDto menuDto);
        Task<bool> AddRangeExcelAsync(CreateExcelMenuDto createExcelMenuDto);
        Task<bool> UpdateAsync(MenuDto menuDto);
        Task<bool> RemoveAsync(int id, int restaurantId);
        Task<MenuRestaurantDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<MenuRestaurantDto>> ListRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int? categoryMenuId, string? menuName);
        Task<List<MenuCustomerByCategoryMenuDto>> ListMenuCustomerByCategoryMenuAsync(int restaurantId);
    }
}
