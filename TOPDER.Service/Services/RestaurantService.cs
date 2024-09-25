using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Dtos.Blog;

namespace TOPDER.Service.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IBlogRepository _blogRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository, IMapper mapper, IBlogRepository blogRepository)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _blogRepository = blogRepository;
        }

        public async Task<Restaurant> AddAsync(CreateRestaurantRequest restaurantRequest)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantRequest);
            return await _restaurantRepository.CreateAndReturnAsync(restaurant);
        }

        public async Task<RestaurantHomeDto> GetHomeItemsAsync()
        {
            var queryable = await _restaurantRepository.QueryableAsync();
            var queryableBlog = await _blogRepository.QueryableAsync();

            var activeBlogs = queryableBlog.Where(x => x.Status == Common_Status.ACTIVE);

            var enabledRestaurants = queryable.Where(r => r.IsBookingEnabled == true);

            var restaurantDtos = enabledRestaurants.Select(r => _mapper.Map<RestaurantDto>(r)).ToList();
            var blogDtos = activeBlogs.Select(b => _mapper.Map<BlogListCustomerDto>(b)).ToList();

            var topBookingRestaurants = restaurantDtos.OrderByDescending(x => x.TotalFeedbacks).Take(6).ToList();
            var topStarRestaurants = restaurantDtos.OrderByDescending(x => x.Star)
                                                   .ThenByDescending(x => x.TotalFeedbacks)
                                                   .Take(6).ToList();
            var newRestaurants = restaurantDtos.OrderByDescending(x => x.ResId).Take(6).ToList();
            var topBlogs = blogDtos.OrderByDescending(x => x.BlogId).Take(6).ToList();

            return new RestaurantHomeDto
            {
                TopBookingRestaurants = topBookingRestaurants,
                TopRatingRestaurant = topStarRestaurants,
                NewRestaurants = newRestaurants,
                Blogs = topBlogs
            };
        }


        public async Task<RestaurantDetailDto> GetItemAsync(int id)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var restaurant = await query.FirstOrDefaultAsync(x => x.Uid == id);

            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy nhà hàng với ID {id}.");
            }

            if (restaurant.IsBookingEnabled == false)
            {
                throw new InvalidOperationException("Nhà hàng này hiện không cho phép đặt chỗ.");
            }

            var restaurantDto = _mapper.Map<RestaurantDetailDto>(restaurant);

            var relateRestaurants = await query
                .Where(x => x.CategoryRestaurantId == restaurant.CategoryRestaurantId
                && x.Uid != id && x.IsBookingEnabled == true)
                .Take(10) 
                .ToListAsync();

            var relateRestaurantDto = _mapper.Map<List<RestaurantDto>>(relateRestaurants);
            restaurantDto.RelateRestaurant = relateRestaurantDto;
            return restaurantDto;
        }



        public async Task<PaginatedList<RestaurantDto>> GetItemsAsync(int pageNumber, int pageSize, string? name, 
            string? address, string? location, int? restaurantCategory, decimal? minPrice, decimal? maxPrice, int? maxCapacity)
        {
            var queryable = await _restaurantRepository.QueryableAsync();

            queryable = queryable.Where(r => r.IsBookingEnabled == true);

            if (!string.IsNullOrEmpty(name))
            {
                queryable = queryable.Where(r => r.NameRes.Contains(name));
            }

            if (!string.IsNullOrEmpty(address))
            {
                queryable = queryable.Where(r => r.Address.Contains(address));
            }

            if (!string.IsNullOrEmpty(location))
            {
                queryable = queryable.Where(r => r.Location.Contains(location));
            }

            if (restaurantCategory.HasValue)
            {
                queryable = queryable.Where(r => r.CategoryRestaurantId == restaurantCategory.Value);
            }

            if (minPrice.HasValue)
            {
                queryable = queryable.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                queryable = queryable.Where(r => r.Price <= maxPrice.Value);
            }

            if (maxCapacity.HasValue)
            {
                queryable = queryable.Where(r => r.MaxCapacity <= maxCapacity.Value);
            }

            var queryDTO = queryable.Select(r => _mapper.Map<RestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public Task<bool> RemoveItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateItemAsync(Restaurant restaurant)
        {
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(restaurant.Uid);
            if (existingRestaurant == null)
            {
                return false;
            }
            return await _restaurantRepository.UpdateAsync(restaurant);
        }

    }
}
