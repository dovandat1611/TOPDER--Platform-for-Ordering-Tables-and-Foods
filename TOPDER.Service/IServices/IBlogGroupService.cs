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
        Task<bool> AddAsync(BlogGroupDto blogGroup);
        Task<bool> UpdateAsync(BlogGroupDto blogGroup);
        Task<bool> RemoveAsync(int id);
        Task<BlogGroupDto> GetItemAsync(int id);
        Task<List<BlogGroupDto>> BlogGroupExistAsync();
        Task<PaginatedList<BlogGroupDto>> ListPagingAsync(int pageNumber, int pageSize, string? blogGroupName);
    }
}
