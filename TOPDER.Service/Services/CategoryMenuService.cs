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
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CategoryMenuService : ICategoryMenuService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryMenuRepository _categoryMenuRepository;
        private readonly IMenuRepository _menuRepository;


        public CategoryMenuService(ICategoryMenuRepository categoryMenuRepository, IMapper mapper, IMenuRepository menuRepository)
        {
            _categoryMenuRepository = categoryMenuRepository;
            _mapper = mapper;
            _menuRepository = menuRepository;
        }
        public async Task<bool> AddAsync(CreateCategoryMenuDto categoryMenuDto)
        {
            var categoryMenu = _mapper.Map<CategoryMenu>(categoryMenuDto);
            return await _categoryMenuRepository.CreateAsync(categoryMenu);
        }

        public async Task<CategoryMenuDto> GetItemAsync(int id, int restaurantId)
        {
            var query = await _categoryMenuRepository.GetByIdAsync(id);
            if (query == null)
            {
                throw new KeyNotFoundException($"Category Menu với id {id} không tồn tại.");
            }
            if (query.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Category Menu với id {id} không thuộc về nhà hàng với id {restaurantId}.");
            }
            var categoryMenuDto = _mapper.Map<CategoryMenuDto>(query);
            return categoryMenuDto;
        }

        public async Task<bool> InvisibleAsync(int id)
        {
            var categoryMenu = await _categoryMenuRepository.GetByIdAsync(id);
            if (categoryMenu == null)
            {
                return false;
            }

            var queryableMenus = await _menuRepository.QueryableAsync();

            var associatedMenus = await queryableMenus
                .Where(m => m.CategoryMenuId == id)
                .ToListAsync();

            if (associatedMenus.Any())
            {
                foreach (var menu in associatedMenus)
                {
                    if(menu.IsVisible == true)
                    {
                        menu.IsVisible = false;
                        await _menuRepository.UpdateAsync(menu);
                    }
                }
            }

            categoryMenu.IsVisible = false; 
            return await _categoryMenuRepository.UpdateAsync(categoryMenu);
        }



        public async Task<List<CategoryMenuDto>> GetAllCategoryMenuAsync(int restaurantId)
        {
            var query = await _categoryMenuRepository.QueryableAsync();

            var categoryMenus = await query.Where(x => x.RestaurantId == restaurantId && x.IsVisible == true)
                .OrderByDescending(x => x.CategoryMenuId)
                .ToListAsync();

            if (categoryMenus == null || !categoryMenus.Any())
            {
                return new List<CategoryMenuDto>();
            }

            var categoryMenusDTO = _mapper.Map<List<CategoryMenuDto>>(categoryMenus);

            return categoryMenusDTO;
        }

        public async Task<PaginatedList<CategoryMenuDto>> ListPagingAsync(int pageNumber, int pageSize, int restaurantId, string? categoryMenuName)
        {
            var queryable = await _categoryMenuRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId && x.IsVisible == true);

            if (!string.IsNullOrEmpty(categoryMenuName))
            {
                query = query.Where(bg => bg.CategoryMenuName != null && bg.CategoryMenuName.Contains(categoryMenuName)); 
            }

            query = query.OrderByDescending(x => x.CategoryMenuId);

            var queryDTO = query.Select(r => _mapper.Map<CategoryMenuDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryMenuDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(CategoryMenuDto categoryMenuDto)
        {
            var existingCategoryMenu = await _categoryMenuRepository.GetByIdAsync(categoryMenuDto.CategoryMenuId);
            if (existingCategoryMenu == null)
            {
                return false;
            }
            existingCategoryMenu.CategoryMenuName = categoryMenuDto.CategoryMenuName;
            return await _categoryMenuRepository.UpdateAsync(existingCategoryMenu);
        }
    }
}
