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
        Task<PaginatedList<RestaurantHomeDTO>> GetAllItemsPagingAsync(int pageNumber, int pageSize);
        Task<IEnumerable<RestaurantHomeDTO>> GetAllItemsAsync();
        Task<RestaurantHomeDTO> GetItemAsync(int id);
        Task<IEnumerable<RestaurantHomeDTO>> SearchItemsByNameAsync(string name);
        Task<IEnumerable<RestaurantHomeDTO>> SearchItemsByAddressAsync(string address);
    }
}
