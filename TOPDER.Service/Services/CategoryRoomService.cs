using AutoMapper;
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

        public Task<CategoryRoomDto> GetItemAsync(int id, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<CategoryRoomDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<CategoryRoomDto>> SearchPagingAsync(int pageNumber, int pageSize, int restaurantId, string categoryRoomName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(CategoryRoomDto categoryRoom)
        {
            throw new NotImplementedException();
        }
    }
}
