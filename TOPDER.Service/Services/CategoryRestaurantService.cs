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
        private readonly IRestaurantRepository _restaurantRepository;


        public CategoryRestaurantService(ICategoryRestaurantRepository categoryRestaurantRepository, IMapper mapper, IRestaurantRepository restaurantRepository)
        {
            _categoryRestaurantRepository = categoryRestaurantRepository;
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<bool> AddAsync(CategoryRestaurantDto categoryRestaurantDto)
        {
            var categoryRestaurant = _mapper.Map<CategoryRestaurant>(categoryRestaurantDto);
            return await _categoryRestaurantRepository.CreateAsync(categoryRestaurant);
        }

        public async Task<CategoryRestaurantDto> UpdateItemAsync(int id)
        {
            var query = await _categoryRestaurantRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category Restaurant với id {id} không tồn tại.");
            var categoryRestaurantDto = _mapper.Map<CategoryRestaurantDto>(query);
            return categoryRestaurantDto;
        }

        public async Task<PaginatedList<CategoryRestaurantViewDto>> ListPagingAsync(int pageNumber, int pageSize, string? categoryRestaurantName)
        {
            var queryable = await _categoryRestaurantRepository.QueryableAsync();

            if (!string.IsNullOrEmpty(categoryRestaurantName))
            {
                queryable = queryable.Where(x => x.CategoryRestaurantName != null && x.CategoryRestaurantName.Contains(categoryRestaurantName));
            }

            queryable = queryable.OrderByDescending(x => x.CategoryRestaurantId);

            var queryDTO = queryable.Select(r => _mapper.Map<CategoryRestaurantViewDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryRestaurantViewDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<List<CategoryRestaurantViewDto>> GetAllCategoryRestaurantAsync()
        {
            var categoryRestaurants = await _categoryRestaurantRepository.GetAllAsync();

            if (categoryRestaurants == null || !categoryRestaurants.Any())
            {
                return new List<CategoryRestaurantViewDto>();
            }

            categoryRestaurants = categoryRestaurants.OrderByDescending(x => x.CategoryRestaurantId);

            var categoryRestaurantsDTO = _mapper.Map<List<CategoryRestaurantViewDto>>(categoryRestaurants);

            return categoryRestaurantsDTO;
        }

        public async Task<bool> UpdateAsync(CategoryRestaurantDto categoryRestaurantDto)
        {
            var existingCategoryRestaurant = await _categoryRestaurantRepository.GetByIdAsync(categoryRestaurantDto.CategoryRestaurantId);
            if (existingCategoryRestaurant == null)
            {
                return false;
            }
            existingCategoryRestaurant.CategoryRestaurantName = categoryRestaurantDto.CategoryRestaurantName;
            return await _categoryRestaurantRepository.UpdateAsync(existingCategoryRestaurant);
        }

        public async Task<List<CategoryRestaurantDto>> CategoryExistAsync()
        {
            try
            {
                var restaurants = await _restaurantRepository.QueryableAsync();
                var existingCategoryIds = await restaurants
                    .Select(r => r.CategoryRestaurantId)
                    .Distinct()
                    .ToListAsync();

                if (!existingCategoryIds.Any())
                {
                    return new List<CategoryRestaurantDto>(); 
                }

                var categories = await _categoryRestaurantRepository.QueryableAsync();
                var existingCategories = categories
                    .Where(c => existingCategoryIds.Contains(c.CategoryRestaurantId))
                    .ToList();

                var categoryDtos = existingCategories.Select(c => _mapper.Map<CategoryRestaurantDto>(c)).ToList();

                return categoryDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving categories.", ex);
            }
        }

    }
}
