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
        private readonly TopderDBContext _context;

        public RestaurantService(IRestaurantRepository restaurantRepository, IMapper mapper, TopderDBContext context)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<Restaurant> AddAsync(CreateRestaurantRequest restaurantRequest)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantRequest);
            return await _restaurantRepository.CreateAsync(restaurant);
        }

        public async Task<PaginatedList<RestaurantHomeDTO>> GetAllItemsPagingAsync(int pageNumber, int pageSize)
        {
            var query = _restaurantRepository.GetAllItems();

            var queryDTO = query.Select(r => _mapper.Map<RestaurantHomeDTO>(r));

            var paginatedDTOs = await PaginatedList<RestaurantHomeDTO>.CreateAsync(
                queryDTO,
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<IEnumerable<RestaurantHomeDTO>> GetAllItemsAsync()
        {
            var restaurants = await _restaurantRepository.GetAllItemsAsync();
            var restaurantsDTO = _mapper.Map<IEnumerable<RestaurantHomeDTO>>(restaurants);
            return restaurantsDTO;
        }

        public async Task<RestaurantHomeDTO> GetItemAsync(int id)
        {
            var restaurant = await _context.Restaurants
                                        .Include(r => r.Reviews)
                                        .FirstOrDefaultAsync(r => r.Uid == id);

            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant with ID {id} not found.");
            }

            return _mapper.Map<RestaurantHomeDTO>(restaurant);
        }

        public async Task<bool> RemoveItemAsync(int id)
        {
            return await _restaurantRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RestaurantHomeDTO>> SearchItemsByAddressAsync(string address)
        {
            var restaurants = await _context.Restaurants
                                    .Where(r => r.Address.Contains(address))
                                    .Include(r => r.Reviews)
                                    .ToListAsync();

            return _mapper.Map<IEnumerable<RestaurantHomeDTO>>(restaurants);
        }

        public async Task<IEnumerable<RestaurantHomeDTO>> SearchItemsByNameAsync(string name)
        {
            var restaurants = await _context.Restaurants
                                    .Where(r => r.NameRes.Contains(name))
                                    .Include(r => r.Reviews)
                                    .ToListAsync();

            return _mapper.Map<IEnumerable<RestaurantHomeDTO>>(restaurants);
        }

        public async Task<bool> UpdateItemAsync(Restaurant restaurant)
        {
            var existingRestaurant = await _context.Restaurants.FindAsync(restaurant.Uid);
            if (existingRestaurant == null)
            {
                return false;
            }
            return await _restaurantRepository.UpdateAsync(restaurant);
        }
    }
}
