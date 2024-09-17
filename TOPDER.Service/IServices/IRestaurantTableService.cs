﻿using System;
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
        Task<bool> UpdateAsync(RestaurantTableDto restaurantTableDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<RestaurantTableRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<RestaurantTableRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, string tableName);
        Task<PaginatedList<RestaurantTableCustomerDto>> GetAvailableTablesAsync(int pageNumber, int pageSize, int restaurantId, TimeSpan TimeReservation, DateTime DateReservation);
    }
}