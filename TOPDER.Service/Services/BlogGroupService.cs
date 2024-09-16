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

        public BlogGroupService(IBlogGroupRepository blogGroupRepository, IMapper mapper)
        {
            _blogGroupRepository = blogGroupRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(string blogGroupName)
        {
            BlogGroup blogGroup = new BlogGroup()
            {
                BloggroupName = blogGroupName,
            };
            return await _blogGroupRepository.CreateAsync(blogGroup);
        }

        public async Task<PaginatedList<BlogGroupDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _blogGroupRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<BlogGroupDto>(r));

            var paginatedDTOs = await PaginatedList<BlogGroupDto>.CreateAsync(
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

        public async Task<PaginatedList<BlogGroupDto>> SearchPagingAsync(int pageNumber, int pageSize, string blogGroupName)
        {
            var queryable = await _blogGroupRepository.QueryableAsync();

            var query = queryable.Where(x => x.BloggroupName.Contains(blogGroupName));

            var queryDTO = query.Select(r => _mapper.Map<BlogGroupDto>(r));

            var paginatedDTOs = await PaginatedList<BlogGroupDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(BlogGroup blogGroup)
        {
            var existingBlogGroup = await _blogGroupRepository.GetByIdAsync(blogGroup.BloggroupId);
            if (existingBlogGroup == null)
            {
                return false;
            }
            return await _blogGroupRepository.UpdateAsync(blogGroup);
        }
    }
}
