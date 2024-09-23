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
using TOPDER.Service.Dtos.Excel;
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

        public async Task<PaginatedList<RestaurantTableCustomerDto>> GetAvailableTablesAsync(
            int pageNumber,
            int pageSize,
            int restaurantId,
            TimeSpan timeReservation,
            DateTime dateReservation)
        {
            var queryable = await _restaurantTableRepository.QueryableAsync();

            var availableTables = queryable
                .Where(table => table.RestaurantId == restaurantId &&
                                table.IsBookingEnabled == true &&
                                !table.OrderTables.Any(orderTable =>
                                    orderTable.Order.DateReservation.Date == dateReservation.Date &&
                                    orderTable.Order.TimeReservation == timeReservation
                                ));

            var queryDTO = availableTables.Select(r => _mapper.Map<RestaurantTableCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantTableCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

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



        public async Task<PaginatedList<RestaurantTableRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _restaurantTableRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<RestaurantTableRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantTableRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
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


        public async Task<PaginatedList<RestaurantTableRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, string tableName)
        {
            var queryable = await _restaurantTableRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId && x.TableName.Contains(tableName));

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
