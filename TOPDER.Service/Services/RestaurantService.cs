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

namespace TOPDER.Service.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository, IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(CreateRestaurantRequest restaurantRequest)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantRequest);
            return await _restaurantRepository.CreateAsync(restaurant);
        }

        public async Task<PaginatedList<RestaurantHomeDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<RestaurantHomeDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantHomeDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<IEnumerable<RestaurantHomeDto>> GetAllItemsAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            var restaurantsDTO = _mapper.Map<IEnumerable<RestaurantHomeDto>>(restaurants);
            return restaurantsDTO;
        }

        public async Task<RestaurantHomeDto> GetItemAsync(int id)
        {
            var queryableRestaurants = await _restaurantRepository.QueryableAsync();

            var restaurant = await queryableRestaurants
                .Include(r => r.Feedbacks) 
                .FirstOrDefaultAsync(r => r.Uid == id);

            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant with ID {id} not found."); 
            }

            return _mapper.Map<RestaurantHomeDto>(restaurant);
        }

        public async Task<bool> RemoveItemAsync(int id)
        {
            return await _restaurantRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RestaurantHomeDto>> SearchItemsByAddressAsync(string address)
        {
            var queryableRestaurants = await _restaurantRepository.QueryableAsync();

            var restaurants = await queryableRestaurants
                .Where(r => r.Address.Contains(address))
                .Include(r => r.Feedbacks)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RestaurantHomeDto>>(restaurants);
        }

        public async Task<IEnumerable<RestaurantHomeDto>> SearchItemsByNameAsync(string name)
        {
            var queryableRestaurants = await _restaurantRepository.QueryableAsync();

            var restaurants = await queryableRestaurants
                .Where(r => r.NameRes.Contains(name))
                .Include(r => r.Feedbacks)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RestaurantHomeDto>>(restaurants);
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
