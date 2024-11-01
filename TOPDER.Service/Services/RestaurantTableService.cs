using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Excel;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IRestaurantTableRepository _restaurantTableRepository;

        public RestaurantTableService(IRestaurantTableRepository restaurantTableRepository, IMapper mapper, IExcelService excelService)
        {
            _restaurantTableRepository = restaurantTableRepository;
            _mapper = mapper;
            _excelService = excelService;
        }
        public async Task<bool> AddAsync(RestaurantTableDto restaurantTableDto)
        {
            var restaurantTable = _mapper.Map<RestaurantTable>(restaurantTableDto);
            return await _restaurantTableRepository.CreateAsync(restaurantTable);
        }

        public async Task<bool> AddRangeExcelAsync(CreateExcelRestaurantTableDto createExcelRestaurantTableDto)
        {
            if (createExcelRestaurantTableDto.File == null || createExcelRestaurantTableDto.File.Length == 0)
            {
                return false;
            }
            try
            {
                var columnConfigurations = new List<ExcelColumnConfiguration>
                {
                    new ExcelColumnConfiguration { ColumnName = "TableName", Position = 1, IsRequired = true },
                    new ExcelColumnConfiguration { ColumnName = "MaxCapacity", Position = 2, IsRequired = true },
                    new ExcelColumnConfiguration { ColumnName = "Description", Position = 3, IsRequired = false },
                    new ExcelColumnConfiguration { ColumnName = "RoomId", Position = 4, IsRequired = false }
                };

                var data = await _excelService.ReadFromExcelAsync(createExcelRestaurantTableDto.File, columnConfigurations);

                var restaurantTables = new List<RestaurantTable>();
                foreach (var row in data)
                {
                    if (row == null ||
                        !row.ContainsKey("TableName") ||
                        !row.ContainsKey("MaxCapacity"))
                    {
                        continue; 
                    }

                    var restaurantTable = new RestaurantTable
                    {
                        RestaurantId = createExcelRestaurantTableDto.RestaurantId,
                        TableName = row["TableName"],
                        MaxCapacity = Convert.ToInt32(row["MaxCapacity"]),
                        Description = row.ContainsKey("Description") ? row["Description"] : null,
                        RoomId = row.ContainsKey("RoomId") && !string.IsNullOrEmpty(row["RoomId"]) ? (int?)Convert.ToInt32(row["RoomId"]) : null,
                        IsBookingEnabled = true,
                    };

                    restaurantTables.Add(restaurantTable);
                }

                if (restaurantTables.Any())
                {
                    await _restaurantTableRepository.CreateRangeAsync(restaurantTables);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task<AvailableTablesDto> GetAvailableTablesAsync(
        //    int restaurantId,
        //    TimeSpan timeReservation,
        //    DateTime dateReservation)
        //{
        //    // Tối ưu hóa truy vấn, chỉ lấy các trường cần thiết
        //    // Lấy truy vấn ban đầu từ repository
        //    var queryableTables = await _restaurantTableRepository.QueryableAsync();

        //    // Thêm điều kiện lọc cho nhà hàng, trạng thái đặt bàn và thời gian đặt bàn
        //    var filteredTables = queryableTables
        //        .Include(x => x.Room)
        //        .Where(table => table.RestaurantId == restaurantId &&
        //                        table.IsBookingEnabled == true &&
        //                        (table.Room == null || table.Room.IsBookingEnabled == true) && // Kiểm tra Room có null hay không
        //                        !table.OrderTables.Any(orderTable =>
        //                            orderTable.Order.DateReservation.Date == dateReservation.Date &&
        //                            orderTable.Order.TimeReservation == timeReservation));

        //    // Áp dụng Select để chỉ lấy những trường cần thiết
        //    var availableTables = await filteredTables
        //        .Select(table => new
        //        {
        //            Table = table,
        //            Room = table.Room
        //        })
        //        .ToListAsync();


        //    // Khởi tạo đối tượng để chứa kết quả
        //    AvailableTablesDto availableTablesDto = new AvailableTablesDto()
        //    {
        //        TablesWithRooms = new List<RestaurantTableIncludeRoomDto>(),
        //        StandaloneTables = new List<RestaurantTableCustomerDto>()
        //    };

        //    // Từ điển để nhóm các bàn theo phòng
        //    var roomsDictionary = new Dictionary<int, RestaurantTableIncludeRoomDto>();

        //    foreach (var entry in availableTables)
        //    {
        //        var table = entry.Table;
        //        var room = entry.Room;

        //        if (room != null)
        //        {
        //            // Nếu phòng tồn tại, kiểm tra xem phòng đã có trong từ điển chưa
        //            if (!roomsDictionary.TryGetValue(room.RoomId, out var existingRoom))
        //            {
        //                // Nếu phòng chưa tồn tại, tạo mới
        //                existingRoom = new RestaurantTableIncludeRoomDto
        //                {
        //                    RestaurantRoom = _mapper.Map<RestaurantRoomDto>(room),
        //                    restaurantTables = new List<RestaurantTableCustomerDto> { _mapper.Map<RestaurantTableCustomerDto>(table) }
        //                };

        //                roomsDictionary.Add(room.RoomId, existingRoom);
        //                availableTablesDto.TablesWithRooms.Add(existingRoom);
        //            }
        //            else
        //            {
        //                // Nếu phòng đã tồn tại, thêm bàn vào danh sách bàn của phòng
        //                existingRoom.restaurantTables.Add(_mapper.Map<RestaurantTableCustomerDto>(table));
        //            }
        //        }
        //        else
        //        {
        //            // Nếu bàn không có phòng, thêm vào danh sách bàn riêng lẻ
        //            availableTablesDto.StandaloneTables.Add(_mapper.Map<RestaurantTableCustomerDto>(table));
        //        }
        //    }
        //    return availableTablesDto;
        //}


        public async Task<RestaurantTableRestaurantDto> GetItemAsync(int id, int restaurantId)
        {
            var table = await _restaurantTableRepository.GetByIdAsync(id);

            if (table == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bàn với Id {id}.");
            }

            if (table.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Bàn với Id {id} không thuộc về nhà hàng với Id {restaurantId}.");
            }

            return _mapper.Map<RestaurantTableRestaurantDto>(table);
        }



        public async Task<bool> IsEnabledBookingAsync(int tableId, int restaurantId, bool isEnabledBooking)
        {
            var existingTable = await _restaurantTableRepository.GetByIdAsync(tableId);
            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bàn với Id {tableId}.");
            }

            if (existingTable.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Bàn với Id {tableId} không thuộc về nhà hàng với Id {restaurantId}.");
            }

            if (isEnabledBooking == existingTable.IsBookingEnabled)
            {
                return false; // Không có sự thay đổi
            }

            existingTable.IsBookingEnabled = isEnabledBooking;
            return await _restaurantTableRepository.UpdateAsync(existingTable);
        }


        public async Task<bool> RemoveAsync(int id, int restaurantId)
        {
            var table = await _restaurantTableRepository.GetByIdAsync(id);

            if (table == null || table.RestaurantId != restaurantId)
            {
                return false;
            }
            return await _restaurantTableRepository.DeleteAsync(id);
        }

        public async Task<List<RestaurantTableRestaurantDto>> GetAvailableTablesAsync(int restaurantId, TimeSpan timeReservation, DateTime dateReservation)
        {
            var queryableTables = await _restaurantTableRepository.QueryableAsync();

            DateTime reservationDateTime = dateReservation.Date + timeReservation;

            var filteredTables = await queryableTables
                .Include(x => x.Room)
                .Include(x => x.TableBookingSchedules)
                .Include(x => x.OrderTables)
                .Where(table => table.RestaurantId == restaurantId &&
                                table.IsBookingEnabled == true &&
                                (table.Room == null || table.Room.IsBookingEnabled == true) &&
                                !table.OrderTables.Any(orderTable =>
                                    orderTable.Order.DateReservation.Date == dateReservation.Date &&
                                    orderTable.Order.TimeReservation == timeReservation) &&
                                !table.TableBookingSchedules.Any(schedule =>
                                    reservationDateTime > schedule.StartTime && reservationDateTime < schedule.EndTime
                                )
                )
                .ToListAsync();

            var restaurantTable = _mapper.Map<List<RestaurantTableRestaurantDto>>(filteredTables);

            return restaurantTable;
        }


        public async Task<PaginatedList<RestaurantTableRestaurantDto>> GetTableListAsync(int pageNumber, int pageSize, int restaurantId, string? tableName)
        {
            var queryable = await _restaurantTableRepository.QueryableAsync();

            // Build the query
            var query = queryable
                .Include(x => x.Room)
                .Where(x => x.RestaurantId == restaurantId);

            // Apply table name filtering only if it has a value
            if (!string.IsNullOrEmpty(tableName))
            {
                query = query.Where(x => x.TableName.Contains(tableName));
            }

            var queryDTO = query.Select(r => _mapper.Map<RestaurantTableRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantTableRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<bool> UpdateAsync(RestaurantTableDto restaurantTableDto)
        {
            var existingRestaurantTable = await _restaurantTableRepository.GetByIdAsync(restaurantTableDto.TableId);
            if (existingRestaurantTable == null || existingRestaurantTable.RestaurantId != restaurantTableDto.RestaurantId)
            {
                return false;
            }
            var restaurantTable = _mapper.Map<RestaurantTable>(restaurantTableDto);
            return await _restaurantTableRepository.UpdateAsync(restaurantTable);
        }


    }
}
