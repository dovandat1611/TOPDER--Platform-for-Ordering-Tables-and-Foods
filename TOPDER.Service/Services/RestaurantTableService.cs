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
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantTableRepository _restaurantTableRepository;

        public RestaurantTableService(IRestaurantTableRepository restaurantTableRepository, IMapper mapper)
        {
            _restaurantTableRepository = restaurantTableRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(RestaurantTableDto restaurantTableDto)
        {
            var restaurantTable = _mapper.Map<RestaurantTable>(restaurantTableDto);
            return await _restaurantTableRepository.CreateAsync(restaurantTable);
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

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
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
            if (existingRestaurantTable == null)
            {
                return false;
            }
            var restaurantTable = _mapper.Map<RestaurantTable>(restaurantTableDto);
            return await _restaurantTableRepository.UpdateAsync(restaurantTable);
        }
    }
}
