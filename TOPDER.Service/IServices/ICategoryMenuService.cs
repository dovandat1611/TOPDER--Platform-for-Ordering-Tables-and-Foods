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
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<CategoryMenuDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<CategoryMenuDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId ,string blogGroupName);
    }
}
