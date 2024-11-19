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
        private readonly IRestaurantTableRepository _restaurantTableRepository;


        public RestaurantRoomService(IRestaurantRoomRepository restaurantRoomRepository, IMapper mapper
            , IExcelService excelService, IRestaurantTableRepository restaurantTableRepository)
        {
            _restaurantRoomRepository = restaurantRoomRepository;
            _mapper = mapper;
            _excelService = excelService;
            _restaurantTableRepository = restaurantTableRepository;
        }

        public async Task<bool> AddAsync(CreateRestaurantRoomDto restaurantRoomDto)
        {
            RestaurantRoom restaurantRoom = new RestaurantRoom()
            {
                RoomId = 0,
                RestaurantId = restaurantRoomDto.RestaurantId,
                CategoryRoomId = restaurantRoomDto.CategoryRoomId,
                MaxCapacity = restaurantRoomDto.MaxCapacity,
                Description = restaurantRoomDto.Description,
                RoomName = restaurantRoomDto.RoomName,
                IsBookingEnabled = true,
                IsVisible = true,
            };
            return await _restaurantRoomRepository.CreateAsync(restaurantRoom);
        }

        public async Task<(bool IsSuccess, string Message)> AddRangeExcelAsync(CreateExcelRestaurantRoomDto createExcelRestaurantRoom)
        {
            if (createExcelRestaurantRoom.File == null || createExcelRestaurantRoom.File.Length == 0)
            {
                return (false, "Tệp Excel không có dữ liệu hoặc không tồn tại.");
            }

            try
            {
                var columnConfigurations = new List<ExcelColumnConfiguration>
        {
            new ExcelColumnConfiguration { ColumnName = "Tên Phòng", Position = 1, IsRequired = true },
            new ExcelColumnConfiguration { ColumnName = "Sức chứa", Position = 2, IsRequired = true },
            new ExcelColumnConfiguration { ColumnName = "Mô tả", Position = 3, IsRequired = false },
        };

                var data = await _excelService.ReadFromExcelAsync(createExcelRestaurantRoom.File, columnConfigurations);

                if (data == null || !data.Any())
                {
                    return (false, "Không có dữ liệu hợp lệ trong tệp Excel.");
                }

                var restaurantRooms = new List<RestaurantRoom>();
                foreach (var row in data)
                {
                    if (row == null || !row.ContainsKey("Tên Phòng") || !row.ContainsKey("Sức chứa"))
                    {
                        return (false, "Một số hàng trong tệp Excel thiếu thông tin bắt buộc như Tên Phòng hoặc Sức chứa.");
                    }

                    var restaurantRoom = new RestaurantRoom
                    {
                        RoomId = 0,
                        RestaurantId = createExcelRestaurantRoom.RestaurantId,
                        RoomName = row["Tên Phòng"],
                        MaxCapacity = Convert.ToInt32(row["Sức chứa"]),
                        Description = row.ContainsKey("Mô tả") ? row["Mô tả"] : null,
                        CategoryRoomId = null,
                        IsBookingEnabled = true,
                        IsVisible = true,
                    };

                    restaurantRooms.Add(restaurantRoom);
                }

                if (restaurantRooms.Any())
                {
                    bool result = await _restaurantRoomRepository.CreateRangeAsync(restaurantRooms);
                    return result
                        ? (true, "Tải lên và lưu trữ dữ liệu thành công.")
                        : (false, "Không thể lưu trữ dữ liệu vào hệ thống.");
                }
                else
                {
                    return (false, "Dữ liệu không hợp lệ hoặc không đủ điều kiện để lưu.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Đã xảy ra lỗi trong quá trình tải lên: {ex.Message}");
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


        public async Task<bool> InvisibleAsync(int id, int restaurantId)
        {
            var restaurantRoom = await _restaurantRoomRepository.GetByIdAsync(id);

            // Kiểm tra sự tồn tại của phòng và khớp restaurantId
            if (restaurantRoom == null || restaurantRoom.RestaurantId != restaurantId)
            {
                return false;
            }

            var queryableTables = await _restaurantTableRepository.QueryableAsync();

            var associatedTables = await queryableTables
                                .Where(t => t.RoomId == id)
                                .ToListAsync();

            if (associatedTables.Any())
            {
                foreach (var table in associatedTables)
                {
                    if(table.IsVisible == true)
                    {
                        table.IsVisible = false;
                        await _restaurantTableRepository.UpdateAsync(table);
                    }
                }
            }

            // Cập nhật trường IsVisible thành false thay vì xóa
            restaurantRoom.IsVisible = false;
            return await _restaurantRoomRepository.UpdateAsync(restaurantRoom);
        }


        public async Task<PaginatedList<RestaurantRoomDto>> GetRoomListAsync(int pageNumber, int pageSize, int restaurantId, int? roomId, string? roomName)
        {
            var queryable = await _restaurantRoomRepository.QueryableAsync();

            // Chỉ truy vấn phòng thuộc nhà hàng cụ thể
            var query = queryable.Where(x => x.RestaurantId == restaurantId && x.IsVisible == true);

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

            query = query.OrderByDescending(x => x.RoomId);

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

        public async Task<bool> UpdateAsync(UpdateRestaurantRoomDto restaurantRoomDto)
        {
            var existingRestaurantRoom = await _restaurantRoomRepository.GetByIdAsync(restaurantRoomDto.RoomId);
            if (existingRestaurantRoom == null)
            {
                return false;
            }
            existingRestaurantRoom.MaxCapacity = restaurantRoomDto.MaxCapacity;
            existingRestaurantRoom.CategoryRoomId = restaurantRoomDto.CategoryRoomId;
            existingRestaurantRoom.RoomName = restaurantRoomDto.RoomName;
            existingRestaurantRoom.Description = restaurantRoomDto.Description;
            return await _restaurantRoomRepository.UpdateAsync(existingRestaurantRoom);
        }
    }
}
