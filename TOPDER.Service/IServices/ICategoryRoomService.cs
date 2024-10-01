using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ICategoryRoomService
    {
        Task<bool> AddAsync(CategoryRoomDto categoryRoom);
        Task<bool> UpdateAsync(CategoryRoomDto categoryRoom);
        Task<bool> RemoveAsync(int id);
        Task<CategoryRoomDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<CategoryRoomDto>> ListPagingAsync(int pageNumber, int pageSize, int restaurantId, string? categoryRoomName);
    }
}
