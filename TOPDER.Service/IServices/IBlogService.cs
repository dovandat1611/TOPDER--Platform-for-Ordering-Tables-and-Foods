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
        Task<UpdateBlogModel> GetItemAsync(int id);
        Task<PaginatedList<BlogAdminDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<BlogAdminDto>> SearchPagingAsync(int pageNumber, int pageSize, int blogGroupId, string blogGroupName);
        Task<PaginatedList<BlogListCustomerDto>> GetBlogCustomerPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<BlogListCustomerDto>> SearchBlogByGroupPagingAsync(int pageNumber, int pageSize, int blogGroupId);
        Task<BlogDetailCustomerDto> GetBlogByIdAsync(int blogId);
        Task<List<NewBlogCustomerDto>> GetNewBlogAsync();

    }
}
