using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Wishlist;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IWishlistService
    {
        Task<bool> AddAsync(WishlistDto wishlistDto);
        Task<bool> RemoveAsync(int id, int customerId);
        Task<PaginatedList<UserWishlistDto>> GetPagingAsync(int pageNumber, int pageSize, int customerId);
    }
}
