using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ICategoryMenuService
    {
        Task<bool> AddAsync(CreateCategoryMenuDto createCategoryMenuDto);
        Task<bool> UpdateAsync(CategoryMenuDto categoryMenuDto);
        Task<bool> InvisibleAsync(int id);
        Task<CategoryMenuDto> GetItemAsync(int id, int restaurantId);
        Task<List<CategoryMenuDto>> GetAllCategoryMenuAsync(int restaurantId);
        Task<PaginatedList<CategoryMenuDto>> ListPagingAsync(int pageNumber, int pageSize, int restaurantId ,string? categoryMenuName);
    }
}
