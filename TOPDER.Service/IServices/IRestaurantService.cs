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
        Task<PaginatedList<RestaurantHomeDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<IEnumerable<RestaurantHomeDto>> GetAllItemsAsync();
        Task<RestaurantHomeDto> GetItemAsync(int id);
        Task<IEnumerable<RestaurantHomeDto>> SearchItemsByNameAsync(string name);
        Task<IEnumerable<RestaurantHomeDto>> SearchItemsByAddressAsync(string address);
    }
}
