using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.TableBookingSchedule;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ITableBookingScheduleService
    {
        Task<bool> AddAsync(CreateTableBookingScheduleDto restaurantTableDto);
        Task<bool> UpdateAsync(TableBookingScheduleDto tableBookingSchedule);
        Task<bool> RemoveAsync(int id);
        Task<TableBookingScheduleDto> GetItemAsync(int id);
        Task<List<TableBookingScheduleViewDto>> GetTableBookingScheduleListAsync(int restaurantId);
    }
}
