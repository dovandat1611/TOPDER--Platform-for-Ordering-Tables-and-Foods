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
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CategoryRestaurantService : ICategoryRestaurantService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRestaurantRepository _categoryRestaurantRepository;

        public CategoryRestaurantService(ICategoryRestaurantRepository categoryRestaurantRepository, IMapper mapper)
        {
            _categoryRestaurantRepository = categoryRestaurantRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(CategoryRestaurantDto categoryRestaurantDto)
        {
            var categoryRestaurant = _mapper.Map<CategoryRestaurant>(categoryRestaurantDto);
            return await _categoryRestaurantRepository.CreateAsync(categoryRestaurant);
        }

        public async Task<CategoryRestaurantDto> GetItemAsync(int id)
        {
            var query = await _categoryRestaurantRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category Restaurant với id {id} không tồn tại.");
            var categoryRestaurantDto = _mapper.Map<CategoryRestaurantDto>(query);
            return categoryRestaurantDto;
        }

        public async Task<PaginatedList<CategoryRestaurantDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _categoryRestaurantRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<CategoryRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryRestaurantDto>.CreateAsync(
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

        public async Task<PaginatedList<CategoryRestaurantDto>> SearchPagingAsync(int pageNumber, int pageSize, string categoryRestaurantName)
        {
            var queryable = await _categoryRestaurantRepository.QueryableAsync();

            var query = queryable.Where(x => x.CategoryRestaurantName != null && x.CategoryRestaurantName.Contains(categoryRestaurantName));

            var queryDTO = query.Select(r => _mapper.Map<CategoryRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(CategoryRestaurantDto categoryRestaurantDto)
        {
            var existingCategoryRestaurant = await _categoryRestaurantRepository.GetByIdAsync(categoryRestaurantDto.CategoryRestaurantId);
            if (existingCategoryRestaurant == null)
            {
                return false;
            }
            var categoryRestaurant = _mapper.Map<CategoryRestaurant>(categoryRestaurantDto);
            return await _categoryRestaurantRepository.UpdateAsync(categoryRestaurant);
        }
    }
}
