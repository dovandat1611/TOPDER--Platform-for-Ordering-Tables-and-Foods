using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class BlogGroupService : IBlogGroupService
    {
        private readonly IMapper _mapper;
        private readonly IBlogGroupRepository _blogGroupRepository;
        private readonly IBlogRepository _blogRepository;

        public BlogGroupService(IBlogGroupRepository blogGroupRepository, IMapper mapper, IBlogRepository blogRepository)
        {
            _blogGroupRepository = blogGroupRepository;
            _mapper = mapper;
            _blogRepository = blogRepository;
        }

        public async Task<bool> AddAsync(BlogGroupDto blogGroupDto)
        {
            var blogGroup = _mapper.Map<BlogGroup>(blogGroupDto);
            return await _blogGroupRepository.CreateAsync(blogGroup);
        }

        public async Task<BlogGroupDto> GetItemAsync(int id)
        {
            var query = await _blogGroupRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"BlogGroup với id {id} không tồn tại.");
            var blogGroupDto = _mapper.Map<BlogGroupDto>(query);
            return blogGroupDto;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var blogGroup = await _blogGroupRepository.GetByIdAsync(id);
            if (blogGroup == null)
            {
                return false;
            }

            var blogs = await _blogRepository.QueryableAsync();
            var relatedBlogs = await blogs
                .Where(b => b.BloggroupId == id)
                .ToListAsync();

            if (relatedBlogs.Any())
            {
                var deleteBlogsResult = await _blogRepository.DeleteRangeAsync(relatedBlogs);
                if (!deleteBlogsResult)
                {
                    return false; 
                }
            }

            var result = await _blogGroupRepository.DeleteAsync(id);
            return result;
        }



        public async Task<PaginatedList<BlogGroupDto>> ListPagingAsync(int pageNumber, int pageSize, string? blogGroupName)
        {
            var queryable = await _blogGroupRepository.QueryableAsync();

            var query = string.IsNullOrEmpty(blogGroupName)
                ? queryable 
                : queryable.Where(x => x.BloggroupName.Contains(blogGroupName));

            query = query.OrderByDescending(x => x.BloggroupId);

            var queryDTO = query.Select(r => _mapper.Map<BlogGroupDto>(r));

            var paginatedDTOs = await PaginatedList<BlogGroupDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<bool> UpdateAsync(BlogGroupDto blogGroupDto)
        {   
            var existingBlogGroup = await _blogGroupRepository.GetByIdAsync(blogGroupDto.BloggroupId);
            if (existingBlogGroup == null)
            {
                return false;
            }
            existingBlogGroup.BloggroupName = blogGroupDto.BloggroupName;
            return await _blogGroupRepository.UpdateAsync(existingBlogGroup);
        }

        public async Task<List<BlogGroupDto>> BlogGroupExistAsync()
        {
                var queryBlog = await _blogRepository.QueryableAsync();

                var existingBlogGroupIds = await queryBlog
                    .Select(r => r.BloggroupId)
                    .Distinct()
                    .ToListAsync();

                var queryBlogGroup = await _blogGroupRepository.QueryableAsync();

                var existingBlogGroups = await queryBlogGroup
                    .Where(bg => existingBlogGroupIds.Contains(bg.BloggroupId))
                    .ToListAsync();

                var queryDTO = existingBlogGroups.Select(bg => _mapper.Map<BlogGroupDto>(bg)).ToList();
                return queryDTO;
        }

        public async Task<List<BlogGroupDto>> GetAllBlogsAsync()
        {
            var blogs = await _blogGroupRepository.GetAllAsync();

            // Kiểm tra dữ liệu null hoặc rỗng trước khi map
            if (blogs == null || !blogs.Any())
            {
                return new List<BlogGroupDto>();
            }

            // Sử dụng AutoMapper để map danh sách blog sang DTO
            var blogsDTO = _mapper.Map<List<BlogGroupDto>>(blogs).ToList();

            return blogsDTO;
        }

    }
}
