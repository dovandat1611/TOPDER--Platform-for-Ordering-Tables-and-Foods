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
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CategoryMenuService : ICategoryMenuService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryMenuRepository _categoryMenuRepository;

        public CategoryMenuService(ICategoryMenuRepository categoryMenuRepository, IMapper mapper)
        {
            _categoryMenuRepository = categoryMenuRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(CategoryMenuDto categoryMenuDto)
        {
            var categoryMenu = _mapper.Map<CategoryMenu>(categoryMenuDto);
            return await _categoryMenuRepository.CreateAsync(categoryMenu);
        }

        public async Task<PaginatedList<CategoryMenuDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var query = await _categoryMenuRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<CategoryMenuDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryMenuDto>.CreateAsync(
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

        public async Task<PaginatedList<CategoryMenuDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, string categoryMenuName)
        {
            var queryable = await _categoryMenuRepository.QueryableAsync();

            var query = queryable.Where(x =>
                x.CategoryMenuName != null && x.CategoryMenuName.Contains(categoryMenuName));

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
            var categoryMenu = _mapper.Map<CategoryMenu>(categoryMenuDto);
            return await _categoryMenuRepository.UpdateAsync(categoryMenu);
        }
    }
}
