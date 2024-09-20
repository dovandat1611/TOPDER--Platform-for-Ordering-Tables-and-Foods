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
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(RoleDto roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            return await _roleRepository.CreateAsync(role);
        }

        public async Task<PaginatedList<RoleDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _roleRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<RoleDto>(r));

            var paginatedDTOs = await PaginatedList<RoleDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(RoleDto roleDto)
        {
            var existingRole = await _roleRepository.GetByIdAsync(roleDto.RoleId);
            if (existingRole == null )
            {
                return false;
            }
            var role = _mapper.Map<Role>(roleDto);
            return await _roleRepository.UpdateAsync(role);
        }
    }
}
