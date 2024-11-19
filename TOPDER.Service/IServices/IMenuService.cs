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
        Task<(bool IsSuccess, string Message)> AddRangeExcelAsync(CreateExcelMenuDto createExcelMenuDto);
        Task<bool> UpdateAsync(MenuDto menuDto);
        Task<bool> InvisibleAsync(int id, int restaurantId);
        Task<bool> IsActiveAsync(int id, string status);
        Task<MenuRestaurantDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<MenuRestaurantDto>> ListRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int? categoryMenuId, string? menuName);
        Task<List<MenuCustomerByCategoryMenuDto>> ListMenuCustomerByCategoryMenuAsync(int restaurantId);
    }
}
