using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ICategoryRestaurantService
    {
        Task<bool> AddAsync(CategoryRestaurantDto categoryRestaurantDto);
        Task<bool> UpdateAsync(CategoryRestaurantDto categoryRestaurantDto);
        Task<bool> RemoveAsync(int id);
        Task<CategoryRestaurantDto> GetItemAsync(int id);
        Task<PaginatedList<CategoryRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<CategoryRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, string categoryRestaurantName);
    }
}
