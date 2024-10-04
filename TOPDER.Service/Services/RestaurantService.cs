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

        public async Task<DiscountAndFeeRestaurant> GetDiscountAndFeeAsync(int restaurantId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            DiscountAndFeeRestaurant discountAndFeeRestaurant = new DiscountAndFeeRestaurant()
            {
                RestaurantId = restaurantId,
                DiscountRestaurant = restaurant.Discount,
                FirstFeePercent = restaurant.FirstFeePercent,
                ReturningFeePercent = restaurant.ReturningFeePercent,
                CancellationFeePercent = restaurant.CancellationFeePercent
            };
            
            return discountAndFeeRestaurant;
        }

        public async Task<DescriptionRestaurant> GetDescriptionAsync(int restaurantId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            DescriptionRestaurant descriptionRestaurant = new DescriptionRestaurant()
            {
                RestaurantId = restaurantId,
                Description = restaurant.Description,
                Subdescription = restaurant.Subdescription
            };

            return descriptionRestaurant;
        }

        public async Task<bool> UpdateDescriptionAsync(int restaurantId, string? description, string? subDescription)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                return false; 
            }

            if (!string.IsNullOrEmpty(description))
            {
                restaurant.Description = description;
            }

            if (!string.IsNullOrEmpty(subDescription))
            {
                restaurant.Subdescription = subDescription;
            }

            return await _restaurantRepository.UpdateAsync(restaurant);
        }



        public async Task<RestaurantHomeDto> GetHomeItemsAsync()
        {
            var queryable = await _restaurantRepository.QueryableAsync();
            var queryableBlog = await _blogRepository.QueryableAsync();

            var activeBlogs = queryableBlog
                .Include(x => x.Bloggroup)
                .Include(x => x.Admin)
                .Where(x => x.Status == Common_Status.ACTIVE);

            var enabledRestaurants = queryable
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Where(r => r.IsBookingEnabled == true);

            var restaurantDtos = enabledRestaurants.Select(r => _mapper.Map<RestaurantDto>(r)).ToList();
            var blogDtos = activeBlogs.Select(b => _mapper.Map<BlogListCustomerDto>(b)).ToList();

            var topBookingRestaurants = restaurantDtos.OrderByDescending(x => x.TotalFeedbacks).Take(6).ToList();
            var topStarRestaurants = restaurantDtos.OrderByDescending(x => x.Star)
                                                   .ThenByDescending(x => x.TotalFeedbacks)
                                                   .Take(6).ToList();
            var newRestaurants = restaurantDtos.OrderByDescending(x => x.Uid).Take(6).ToList();
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

            var restaurant = await query
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Include(x => x.Images)
                .Include(x => x.Menus)
                .FirstOrDefaultAsync(x => x.Uid == id);

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
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
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

            queryable = queryable
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Where(r => r.IsBookingEnabled == true);

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
                queryable = queryable.Where(r => r.MaxCapacity >= maxCapacity.Value);
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

        public async Task<bool> UpdateDiscountAndFeeAsync(int restaurantId, decimal? discountPrice, decimal? firstFeePercent, decimal? returningFeePercent, decimal? cancellationFeePercent)
        {
            // Lấy thông tin nhà hàng theo restaurantId
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                // Nếu không tìm thấy nhà hàng, trả về false
                return false;
            }

            int count = 0;

            // Kiểm tra và cập nhật giá trị discountPrice nếu hợp lệ
            if (discountPrice.HasValue && discountPrice > 0 && discountPrice <= 100)
            {
                if (restaurant.Discount != discountPrice)
                {
                    restaurant.Discount = discountPrice;
                    count++;
                }
            }

            // Kiểm tra và cập nhật giá trị firstFeePercent nếu hợp lệ
            if (firstFeePercent.HasValue && firstFeePercent > 0 && firstFeePercent <= 100)
            {
                if (restaurant.FirstFeePercent != firstFeePercent)
                {
                    restaurant.FirstFeePercent = firstFeePercent;
                    count++;
                }
            }

            // Kiểm tra và cập nhật giá trị returningFeePercent nếu hợp lệ
            if (returningFeePercent.HasValue && returningFeePercent > 0 && returningFeePercent <= 100)
            {
                if (restaurant.ReturningFeePercent != returningFeePercent)
                {
                    restaurant.ReturningFeePercent = returningFeePercent;
                    count++;
                }
            }

            // Kiểm tra và cập nhật giá trị cancellationFeePercent nếu hợp lệ
            if (cancellationFeePercent.HasValue && cancellationFeePercent > 0 && cancellationFeePercent <= 100)
            {
                if (restaurant.CancellationFeePercent != cancellationFeePercent)
                {
                    restaurant.CancellationFeePercent = cancellationFeePercent;
                    count++;
                }
            }

            // Chỉ cập nhật nếu có thay đổi (count > 0)
            if (count > 0)
            {
                return await _restaurantRepository.UpdateAsync(restaurant);
            }

            return false;
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
