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
        Task<bool> AddAsync(CreateRestaurantRequest restaurant);
        Task<bool> UpdateItemAsync(Restaurant restaurant);
        Task<bool> RemoveItemAsync(int id);
        Task<RestaurantDetailDto> GetItemAsync(int id);
        Task<PaginatedList<RestaurantHomeDto>> GetItemsAsync(int pageNumber, int pageSize, string? name, string? address,
            string? location, int? restaurantCategory, decimal? minPrice, decimal? maxPrice, int? maxCapacity);
    }
}
