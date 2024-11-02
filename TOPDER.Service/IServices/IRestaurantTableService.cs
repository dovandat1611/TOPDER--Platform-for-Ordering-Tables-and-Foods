using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IRestaurantTableService
    {
        Task<bool> AddAsync(RestaurantTableDto restaurantTableDto);
        Task<bool> AddRangeExcelAsync(CreateExcelRestaurantTableDto createExcelRestaurantTableDto);
        Task<bool> UpdateAsync(RestaurantTableDto restaurantTableDto);
        Task<bool> InvisibleAsync(int id, int restaurantId);
        Task<RestaurantTableRestaurantDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<RestaurantTableRestaurantDto>> GetTableListAsync(int pageNumber, int pageSize, int restaurantId, string? tableName);

        Task<List<RestaurantTableRestaurantDto>> GetAvailableTablesAsync(int restaurantId, TimeSpan TimeReservation, DateTime DateReservation);

        //Task<AvailableTablesDto> GetAvailableTablesAsync(int restaurantId, TimeSpan TimeReservation, DateTime DateReservation);
        Task<bool> IsEnabledBookingAsync(int tableId, int restaurantId, bool isEnabledBooking);
    }
}
