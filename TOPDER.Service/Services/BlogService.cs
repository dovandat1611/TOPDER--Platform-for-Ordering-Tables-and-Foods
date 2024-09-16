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

        public async Task<PaginatedList<BlogAdminDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _blogRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<BlogAdminDto>(r));

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
