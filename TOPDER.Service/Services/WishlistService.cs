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
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Wishlist;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IMapper _mapper;
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository, IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(WishlistDto wishlistDto)
        {
            var queryable = await _wishlistRepository.QueryableAsync();
            var query = queryable.Where(x => x.CustomerId == wishlistDto.CustomerId && x.RestaurantId == wishlistDto.RestaurantId);

            if (await query.AnyAsync())
            {
                return false; 
            }

            var wishlist = _mapper.Map<Wishlist>(wishlistDto);
            return await _wishlistRepository.CreateAsync(wishlist);
        }


        public async Task<PaginatedList<UserWishlistDto>> GetPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var queryable = await _wishlistRepository.QueryableAsync();

            var query = queryable
                .Include(x => x.Restaurant)
                .ThenInclude(r => r.Feedbacks)
                .Include(x => x.Restaurant)
                .ThenInclude(r => r.CategoryRestaurant)
                .Where(x => x.CustomerId == customerId);

            var queryDTO = query.Select(r => _mapper.Map<UserWishlistDto>(r));

            var paginatedDTOs = await PaginatedList<UserWishlistDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id, int customerId)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(id);
            if (wishlist == null || wishlist.CustomerId != customerId)
            {
                return false; 
            }
            return await _wishlistRepository.DeleteAsync(id);
        }

    }
}
