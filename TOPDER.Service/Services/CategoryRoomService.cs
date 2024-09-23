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
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CategoryRoomService : ICategoryRoomService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRoomRepository _categoryRoomRepository;

        public CategoryRoomService(ICategoryRoomRepository categoryRoomRepository, IMapper mapper)
        {
            _categoryRoomRepository = categoryRoomRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(CategoryRoomDto categoryRoom)
        {
            var category = _mapper.Map<CategoryRoom>(categoryRoom);
            return await _categoryRoomRepository.CreateAsync(category);
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

        public async Task<PaginatedList<CategoryRoomDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _categoryRoomRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<CategoryRoomDto>(r));

            var paginatedDTOs = await PaginatedList<CategoryRoomDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedList<CategoryRoomDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, string categoryRoomName)
        {
            var queryable = await _categoryRoomRepository.QueryableAsync();

            var query = queryable.Where(x =>
                x.CategoryName != null && x.CategoryName.Contains(categoryRoomName) && x.RestaurantId == restaurantId);

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
