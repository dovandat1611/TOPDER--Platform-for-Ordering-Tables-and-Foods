using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Excel;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class RestaurantRoomService : IRestaurantRoomService
    {
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IRestaurantRoomRepository _restaurantRoomRepository;

        public RestaurantRoomService(IRestaurantRoomRepository restaurantRoomRepository, IMapper mapper, IExcelService excelService)
        {
            _restaurantRoomRepository = restaurantRoomRepository;
            _mapper = mapper;
            _excelService = excelService;
        }

        public async Task<bool> AddAsync(RestaurantRoomDto restaurantRoomDto)
        {
            var restaurantRoom = _mapper.Map<RestaurantRoom>(restaurantRoomDto);
            return await _restaurantRoomRepository.CreateAsync(restaurantRoom);
        }

        public async Task<bool> AddRangeExcelAsync(CreateExcelRestaurantRoomDto createExcelRestaurantRoom)
        {
            if (createExcelRestaurantRoom.File == null || createExcelRestaurantRoom.File.Length == 0)
            {
                return false;
            }

            try
            {
                var columnConfigurations = new List<ExcelColumnConfiguration>
        {
            new ExcelColumnConfiguration { ColumnName = "RoomName", Position = 1, IsRequired = true },
            new ExcelColumnConfiguration { ColumnName = "MaxCapacity", Position = 2, IsRequired = true },
            new ExcelColumnConfiguration { ColumnName = "Description", Position = 3, IsRequired = false },
            new ExcelColumnConfiguration { ColumnName = "CategoryRoomId", Position = 4, IsRequired = false }
        };

                var data = await _excelService.ReadFromExcelAsync(createExcelRestaurantRoom.File, columnConfigurations);

                var restaurantRooms = new List<RestaurantRoom>();
                foreach (var row in data)
                {
                    // Check if the row is valid
                    if (row == null || !row.ContainsKey("RoomName") || !row.ContainsKey("MaxCapacity"))
                    {
                        continue; // Skip invalid rows
                    }

                    var restaurantRoom = new RestaurantRoom
                    {
                        RestaurantId = createExcelRestaurantRoom.RestaurantId,
                        RoomName = row["RoomName"],
                        MaxCapacity = Convert.ToInt32(row["MaxCapacity"]),
                        Description = row.ContainsKey("Description") ? row["Description"] : null,
                        CategoryRoomId = row.ContainsKey("CategoryRoomId") && !string.IsNullOrEmpty(row["CategoryRoomId"])
                                        ? Convert.ToInt32(row["CategoryRoomId"]) 
                                        : (int?)null, 
                        IsBookingEnabled = true,
                    };

                    restaurantRooms.Add(restaurantRoom);
                }

                if (restaurantRooms.Any())
                {
                    await _restaurantRoomRepository.CreateRangeAsync(restaurantRooms);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<RestaurantRoomDto> GetItemAsync(int id, int restaurantId)
        {
            var table = await _restaurantRoomRepository.GetByIdAsync(id);

            if (table == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bàn với Id {id}.");
            }

            if (table.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Bàn với Id {id} không thuộc về nhà hàng với Id {restaurantId}.");
            }

            return _mapper.Map<RestaurantRoomDto>(table);
        }

        public async Task<bool> IsEnabledBookingAsync(int roomId, int restaurantId, bool isEnabledBooking)
        {
            var existingRoom = await _restaurantRoomRepository.GetByIdAsync(roomId);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy phòng với Id {roomId}.");
            }

            if (existingRoom.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Phòng với Id {roomId} không thuộc về nhà hàng với Id {restaurantId}.");
            }

            if (isEnabledBooking == existingRoom.IsBookingEnabled)
            {
                return false; // Không có sự thay đổi
            }

            existingRoom.IsBookingEnabled = isEnabledBooking;
            return await _restaurantRoomRepository.UpdateAsync(existingRoom);
        }


        public async Task<bool> RemoveAsync(int id, int restaurantId)
        {
            var restaurantRoom = await _restaurantRoomRepository.GetByIdAsync(id);

            if (restaurantRoom == null || restaurantRoom.RestaurantId != restaurantId)
            {
                return false;
            }
            return await _restaurantRoomRepository.DeleteAsync(id);
        }

        public async Task<PaginatedList<RestaurantRoomDto>> GetRoomListAsync(int pageNumber, int pageSize, int restaurantId, int? roomId, string? roomName)
        {
            var queryable = await _restaurantRoomRepository.QueryableAsync();

            // Chỉ truy vấn phòng thuộc nhà hàng cụ thể
            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            // Kiểm tra nếu roomId có giá trị hợp lệ
            if (roomId.HasValue && roomId.Value != 0)
            {
                query = query.Where(x => x.RoomId == roomId.Value);
            }

            // Kiểm tra tên phòng
            if (!string.IsNullOrEmpty(roomName))
            {
                query = query.Where(x => x.RoomName.Contains(roomName));
            }

            // Chọn các DTO từ các thực thể
            var queryDTO = query.Select(r => _mapper.Map<RestaurantRoomDto>(r));

            // Tạo danh sách phân trang
            var paginatedDTOs = await PaginatedList<RestaurantRoomDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(RestaurantRoomDto restaurantRoomDto)
        {
            var existingRestaurantRoom = await _restaurantRoomRepository.GetByIdAsync(restaurantRoomDto.RoomId);
            if (existingRestaurantRoom == null || existingRestaurantRoom.RestaurantId != existingRestaurantRoom.RestaurantId)
            {
                return false;
            }
            var restaurantRoom = _mapper.Map<RestaurantRoom>(restaurantRoomDto);
            return await _restaurantRoomRepository.UpdateAsync(restaurantRoom);
        }
    }
}
