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
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CategoryRoomService : ICategoryRoomService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRoomRepository _categoryRoomRepository;
        private readonly IRestaurantRoomRepository _restaurantRoomRepository;
        private readonly IRestaurantTableRepository _restaurantTableRepository;

        public CategoryRoomService(ICategoryRoomRepository categoryRoomRepository, IMapper mapper, IRestaurantRoomRepository restaurantRoomRepository, IRestaurantTableRepository restaurantTableRepository)
        {
            _categoryRoomRepository = categoryRoomRepository;
            _mapper = mapper;
            _restaurantRoomRepository = restaurantRoomRepository;
            _restaurantTableRepository = restaurantTableRepository;
        }
        public async Task<bool> AddAsync(CategoryRoomDto categoryRoom)
        {
            var category = _mapper.Map<CategoryRoom>(categoryRoom);
            return await _categoryRoomRepository.CreateAsync(category);
        }

        public async Task<List<CategoryRoomDto>> GetAllCategoryRoomAsync(int restaurantId)
        {
            var query = await _categoryRoomRepository.QueryableAsync();

            var categoryRooms = await query.Where(x => x.RestaurantId == restaurantId && x.IsVisible == true)
                .OrderByDescending(x => x.CategoryRoomId)
                .ToListAsync();

            if (categoryRooms == null || !categoryRooms.Any())
            {
                return new List<CategoryRoomDto>();
            }

            var categoryRoomsDTO = _mapper.Map<List<CategoryRoomDto>>(categoryRooms);

            return categoryRoomsDTO;
        }

        public async Task<CategoryRoomDto> GetItemAsync(int id, int restaurantId)
        {
            var query = await _categoryRoomRepository.GetByIdAsync(id);
            if (query == null)
            {
                throw new KeyNotFoundException($"Category Room với id {id} không tồn tại.");
            }
            if (query.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Category Room với id {id} không thuộc về nhà hàng với id {restaurantId}.");
            }
            var categoryRoomDto = _mapper.Map<CategoryRoomDto>(query);
            return categoryRoomDto;
        }

        public async Task<bool> InvisibleAsync(int id)
        {
            var categoryRoom = await _categoryRoomRepository.GetByIdAsync(id);
            if (categoryRoom == null)
            {
                return false;
            }

            var allRooms = await _restaurantRoomRepository.QueryableAsync();
            var associatedRooms = allRooms.Where(r => r.CategoryRoomId == id).ToList();

            if (associatedRooms.Any())
            {
                foreach (var room in associatedRooms)
                {
                    var allTables = await _restaurantTableRepository.QueryableAsync();
                    var associatedTables = allTables.Where(t => t.RoomId == room.RoomId).ToList();

                    // Cập nhật các bảng (table) thành IsVisible = false
                    foreach (var table in associatedTables)
                    {
                        if(table.IsVisible == true)
                        {
                            table.IsVisible = false;
                            await _restaurantTableRepository.UpdateAsync(table);
                        }
                    }

                    if(room.IsVisible == true)
                    {
                        room.IsVisible = false;
                        await _restaurantRoomRepository.UpdateAsync(room);
                    }
                }
            }

            // Cập nhật category room thành IsVisible = false
            categoryRoom.IsVisible = false;
            return await _categoryRoomRepository.UpdateAsync(categoryRoom);
        }


        public async Task<PaginatedList<CategoryRoomDto>> ListPagingAsync(int pageNumber, int pageSize, int restaurantId, string? categoryRoomName)
        {
            var queryable = await _categoryRoomRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId && x.IsVisible == true);

            if (!string.IsNullOrEmpty(categoryRoomName))
            {
                query = query.Where(x => x.CategoryName != null && x.CategoryName.Contains(categoryRoomName));
            }

            query = query.OrderByDescending(x => x.CategoryRoomId);

            var queryDTO = query.Select(r => _mapper.Map<CategoryRoomDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryRoomDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<bool> UpdateAsync(CategoryRoomDto categoryRoom)
        {
            var existingCategoryRoom = await _categoryRoomRepository.GetByIdAsync(categoryRoom.CategoryRoomId);
            if (existingCategoryRoom == null)
            {
                return false;
            }
            var category = _mapper.Map<CategoryRoom>(categoryRoom);
            return await _categoryRoomRepository.UpdateAsync(category);
        }
    }
}
