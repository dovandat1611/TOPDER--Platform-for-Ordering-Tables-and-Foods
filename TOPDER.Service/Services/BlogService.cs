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
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class BlogService : IBlogService
    {
        private readonly IMapper _mapper;
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogGroupRepository _blogGroupRepository;


        public BlogService(IBlogRepository blogRepository, IMapper mapper, IBlogGroupRepository blogGroupRepository)
        {
            _blogRepository = blogRepository;
            _mapper = mapper;
            _blogGroupRepository = blogGroupRepository;
        }

        public async Task<bool> AddAsync(CreateBlogModel createBlogModel)
        {
            var blog = _mapper.Map<Blog>(createBlogModel);
            return await _blogRepository.CreateAsync(blog);
        }

        public async Task<BlogDetailDto> GetBlogByIdAsync(int blogId)
        {
            var queryBlog = await _blogRepository.QueryableAsync();
            var queryBlogGroup = await _blogGroupRepository.QueryableAsync();

            var existingBlog = await queryBlog
                .Include(x => x.Admin)
                .Include(x => x.Bloggroup)
                .FirstOrDefaultAsync(x => x.BlogId == blogId && x.Status == Common_Status.ACTIVE);

            if (existingBlog == null)
            {
                throw new KeyNotFoundException($"Blog với ID {blogId} không tồn tại.");
            }

            var recentBlogs = await queryBlog
                .Include(x => x.Admin)
                .Include(x => x.Bloggroup)
                .Where(x => x.Status == Common_Status.ACTIVE)
                .OrderByDescending(x => x.BlogId)
                .Take(3)
                .ToListAsync();


            var existingBlogGroupIds = await queryBlog
                .Select(r => r.BloggroupId)
                .Distinct()
                .ToListAsync();

            var existingBlogGroups = await queryBlogGroup
                .Where(bg => existingBlogGroupIds.Contains(bg.BloggroupId))
                .ToListAsync();

            var blogDetail = _mapper.Map<BlogDetailCustomerDto>(existingBlog);
            var newBlogs = _mapper.Map<IEnumerable<NewBlogCustomerDto>>(recentBlogs).ToList();  
            var blogGroupHashtags = _mapper.Map<IEnumerable<BlogGroupDto>>(existingBlogGroups).ToList(); 

            // Constructing the DTO
            BlogDetailDto blogDetailDto = new BlogDetailDto
            {
                blogListCustomer = blogDetail,  
                newBlogCustomers = newBlogs,
                blogGroups = blogGroupHashtags
            };

            return blogDetailDto;
        }

        public async Task<UpdateBlogModel> GetUpdateItemAsync(int id)
        {
            var query = await _blogRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Blog với id {id} không tồn tại.");
            var updateBlogModel = _mapper.Map<UpdateBlogModel>(query);
            return updateBlogModel;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return false;
            }
            var result = await _blogRepository.DeleteAsync(id);
            return result;
        }

        public async Task<PaginatedList<BlogListCustomerDto>> CustomerBlogListAsync(int pageNumber, int pageSize, int? blogGroupId, string? title)
        {
            var query = await _blogRepository.QueryableAsync();

            var blogs = query.Where(x => x.Status != null &&
                                         x.Status.Equals(Common_Status.ACTIVE));

            if (blogGroupId.HasValue && blogGroupId > 0)
            {
                blogs = blogs.Where(b => b.BloggroupId == blogGroupId);
            }

            if (!string.IsNullOrEmpty(title))
            {
                blogs = blogs.Where(x => x.Title != null && x.Title.Contains(title));
            }

            blogs = blogs
                .Include(x => x.Bloggroup)
                .Include(x => x.Admin)
                .OrderByDescending(x => x.BlogId);

            var queryDTO = blogs.Select(r => _mapper.Map<BlogListCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<BlogListCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<PaginatedList<BlogAdminDto>> AdminBlogListAsync(int pageNumber, int pageSize, int? blogGroupId, string? title)
        {
            var query = await _blogRepository.QueryableAsync();

            if (blogGroupId.HasValue && blogGroupId > 0)
            {
                query = query.Where(b => b.BloggroupId == blogGroupId);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title != null && b.Title.Contains(title));
            }

            query = query
                .Include(x => x.Bloggroup)
                .Include(x => x.Admin)
                .OrderByDescending(b => b.BlogId);

            var queryDTO = query.Select(r => _mapper.Map<BlogAdminDto>(r));

            var paginatedDTOs = await PaginatedList<BlogAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<bool> UpdateAsync(UpdateBlogModel updateBlogModel)
        {
            var existingBlog = await _blogRepository.GetByIdAsync(updateBlogModel.BlogId);
            if (existingBlog == null)
            {
                return false;
            }
            var blog = _mapper.Map<Blog>(updateBlogModel);
            return await _blogRepository.UpdateAsync(blog);
        }
    }
}
