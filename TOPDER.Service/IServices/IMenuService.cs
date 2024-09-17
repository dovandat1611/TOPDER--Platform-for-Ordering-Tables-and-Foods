﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IMenuService
    {
        Task<bool> AddAsync(MenuDto menuDto);
        Task<bool> UpdateAsync(MenuDto menuDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<MenuRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<MenuRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, int categoryMenuId, string menuName);
        Task<PaginatedList<MenuCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId);
        //Task<PaginatedList<MenuCustomerDto>> SearchByCategoryPagingAsync(int pageNumber, int pageSize, int restaurantId, int categoryMenuId);
    }
}