using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMapper _mapper;
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(MenuDto menuDto)
        {
            var menu = _mapper.Map<Menu>(menuDto);
            return await _menuRepository.CreateAsync(menu);
        }

        public async Task<PaginatedList<MenuCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _menuRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<MenuCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<MenuCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<PaginatedList<MenuRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _menuRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<MenuRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<MenuRestaurantDto>.CreateAsync(
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

        public async Task<PaginatedList<MenuRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, int categoryMenuId, string menuName)
        {
            var queryable = await _menuRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            if (categoryMenuId > 0)
            {
                query = query.Where(x => x.CategoryMenuId == categoryMenuId);
            }

            if (!string.IsNullOrEmpty(menuName))
            {
                query = query.Where(x => x.DishName != null && x.DishName.Contains(menuName));
            }

            var queryDTO = query.Select(r => _mapper.Map<MenuRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<MenuRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(MenuDto menuDto)
        {
            var existingMenu = await _menuRepository.GetByIdAsync(menuDto.MenuId);
            if (existingMenu == null)
            {
                return false;
            }
            var menu = _mapper.Map<Menu>(menuDto);
            return await _menuRepository.UpdateAsync(menu);
        }
    }
}
