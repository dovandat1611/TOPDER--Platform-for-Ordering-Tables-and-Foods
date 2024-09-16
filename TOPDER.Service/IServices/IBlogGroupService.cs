using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IBlogGroupService
    {
        Task<bool> AddAsync(string blogGroupName);
        Task<bool> UpdateAsync(BlogGroup blogGroup);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<BlogGroupDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<BlogGroupDto>> SearchPagingAsync(int pageNumber, int pageSize, string blogGroupName);
    }
}
