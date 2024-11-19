using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IRestaurantRoomService
    {
        Task<bool> AddAsync(CreateRestaurantRoomDto restaurantRoomDto);
        Task<(bool IsSuccess, string Message)> AddRangeExcelAsync(CreateExcelRestaurantRoomDto createExcelRestaurantRoom);
        Task<bool> UpdateAsync(UpdateRestaurantRoomDto restaurantRoomDto);
        Task<bool> InvisibleAsync(int id, int restaurantId);
        Task<RestaurantRoomDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<RestaurantRoomDto>> GetRoomListAsync(int pageNumber, int pageSize, int restaurantId, int? roomId, string? roomName);
        Task<bool> IsEnabledBookingAsync(int roomId, int restaurantId, bool isEnabledBooking);
    }
}
