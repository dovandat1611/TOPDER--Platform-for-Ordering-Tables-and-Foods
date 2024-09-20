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
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class BlogService : IBlogService
    {
        private readonly IMapper _mapper;
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(CreateBlogModel createBlogModel)
        {
            var blog = _mapper.Map<Blog>(createBlogModel);
            return await _blogRepository.CreateAsync(blog);
        }

        public async Task<BlogDetailCustomerDto> GetBlogByIdAsync(int blogId)
        {
            var existingBlog = await _blogRepository.GetByIdAsync(blogId);
            if (existingBlog == null)
            {
                throw new KeyNotFoundException($"Blog với ID {blogId} không tồn tại.");
            }
            return _mapper.Map<BlogDetailCustomerDto>(existingBlog);
        }


        public async Task<PaginatedList<BlogListCustomerDto>> GetBlogCustomerPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _blogRepository.QueryableAsync();

            var blogs = query.Where(x => x.Status != null && x.Status.Equals(Common_Status.ACTIVE))
                             .OrderByDescending(x => x.BlogId);

            var queryDTO = blogs.Select(r => _mapper.Map<BlogListCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<BlogListCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<UpdateBlogModel> GetItemAsync(int id)
        {
            var query = await _blogRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Blog với id {id} không tồn tại.");
            var updateBlogModel = _mapper.Map<UpdateBlogModel>(query);
            return updateBlogModel;
        }

        public async Task<List<NewBlogCustomerDto>> GetNewBlogAsync()
        {
            var query = await _blogRepository.QueryableAsync();

            var blogs = query.Where(x => x.Status != null && x.Status.Equals(Common_Status.ACTIVE))
                .OrderByDescending(x => x.BlogId).Take(3);

            var queryDTO = blogs.Select(r => _mapper.Map<NewBlogCustomerDto>(r));

            return await queryDTO.ToListAsync();
        }

        public async Task<PaginatedList<BlogAdminDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _blogRepository.QueryableAsync();

            var blogs = query.OrderByDescending(x => x.BlogId);

            var queryDTO = blogs.Select(r => _mapper.Map<BlogAdminDto>(r));

            var paginatedDTOs = await PaginatedList<BlogAdminDto>.CreateAsync(
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

        public async Task<PaginatedList<BlogListCustomerDto>> SearchBlogByGroupPagingAsync(int pageNumber, int pageSize, int blogGroupId)
        {
            var query = await _blogRepository.QueryableAsync();

            var blogs = query.Where(x => x.Status != null && x.Status.Equals(Common_Status.ACTIVE) && x.BloggroupId == blogGroupId)
                             .OrderByDescending(x => x.BlogId);

            var queryDTO = blogs.Select(r => _mapper.Map<BlogListCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<BlogListCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<PaginatedList<BlogAdminDto>> SearchPagingAsync(int pageNumber, int pageSize, int blogGroupId, string blogGroupName)
        {
            var query = await _blogRepository.QueryableAsync();

            if (blogGroupId > 0)
            {
                query = query.Where(b => b.BloggroupId == blogGroupId);
            }

            if (!string.IsNullOrEmpty(blogGroupName))
            {
                query = query.Where(b => b.Bloggroup != null &&
                                         b.Bloggroup.BloggroupName.Contains(blogGroupName));
            }

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
