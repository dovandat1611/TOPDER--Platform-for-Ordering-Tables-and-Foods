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
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IMapper _mapper;
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(DiscountDto discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            return await _discountRepository.CreateAsync(discount);
        }

        public async Task<PaginatedList<DiscountDto>> GetAvailableDiscountsAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _discountRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId
                                             && x.Quantity > 0
                                             && x.IsActive == true
                                             && x.StartDate <= DateTime.Now
                                             && x.EndDate >= DateTime.Now);

            var queryDTO = query.Select(r => _mapper.Map<DiscountDto>(r));

            var paginatedDTOs = await PaginatedList<DiscountDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<DiscountDto> GetItemAsync(int id, int restaurantId)
        {
            var query = await _discountRepository.GetByIdAsync(id);
            if (query == null)
            {
                throw new KeyNotFoundException($"Discount với id {id} không tồn tại.");
            }
            if (query.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Discount với id {id} không thuộc về nhà hàng với id {restaurantId}.");
            }
            var discount = _mapper.Map<DiscountDto>(query);
            return discount;
        }

        public async Task<PaginatedList<DiscountDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _discountRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId).OrderByDescending(x => x.DiscountId);

            var queryDTO = query.Select(r => _mapper.Map<DiscountDto>(r));

            var paginatedDTOs = await PaginatedList<DiscountDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id, int restaurantId)
        {
            var feedback = await _discountRepository.GetByIdAsync(id);
            if (feedback == null || feedback.RestaurantId != restaurantId)
            {
                return false;
            }
            var result = await _discountRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> UpdateAsync(DiscountDto discountDto)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(discountDto.DiscountId);
            if (existingDiscount == null || discountDto.RestaurantId != existingDiscount.RestaurantId)
            {
                return false;
            }
            var discount = _mapper.Map<Discount>(discountDto);
            return await _discountRepository.UpdateAsync(discount);
        }
    }
}
