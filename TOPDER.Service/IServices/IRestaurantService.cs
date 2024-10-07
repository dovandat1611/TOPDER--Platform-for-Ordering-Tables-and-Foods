using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Repository.Entities;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IRestaurantService
    {
        Task<Restaurant> AddAsync(CreateRestaurantRequest restaurant);
        Task<bool> UpdateItemAsync(Restaurant restaurant);
        Task<bool> RemoveItemAsync(int id);
        Task<RestaurantDetailDto> GetItemAsync(int id);
        Task<RestaurantHomeDto> GetHomeItemsAsync();
        Task<PaginatedList<RestaurantDto>> GetItemsAsync(int pageNumber, int pageSize, string? name, string? address,
            string? location, int? restaurantCategory, decimal? minPrice, decimal? maxPrice, int? maxCapacity);

        // DISCOUNT AND FEE
        Task<DiscountAndFeeRestaurant> GetDiscountAndFeeAsync(int restaurantId);
        Task<bool> UpdateDiscountAndFeeAsync(int restaurantId, decimal? discountPrice, decimal? firstFeePercent, decimal? returningFeePercent, decimal? cancellationFeePercent);


        // DESCRIPTION 
        Task<bool> UpdateDescriptionAsync(int restaurantId, string? description, string? subDescription);
        Task<DescriptionRestaurant> GetDescriptionAsync(int restaurantId);

        // ISABLE BOOKING 
        Task<bool> IsEnabledBookingAsync(int id, bool isEnabledBooking);

    }
}
