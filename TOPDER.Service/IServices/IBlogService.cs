using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IBlogService
    {
        Task<bool> AddAsync(CreateBlogModel createBlogModel);
        Task<bool> UpdateAsync(UpdateBlogModel updateBlogModel);
        Task<bool> RemoveAsync(int id);
        Task<UpdateBlogModel> GetUpdateItemAsync(int id);
        Task<PaginatedList<BlogAdminDto>> AdminBlogListAsync(int pageNumber, int pageSize, int? blogGroupId, string? title);
        Task<PaginatedList<BlogListCustomerDto>> CustomerBlogListAsync(int pageNumber, int pageSize, int? blogGroupId, string? title);
        Task<BlogDetailDto> GetBlogByIdAsync(int blogId);
    }
}
