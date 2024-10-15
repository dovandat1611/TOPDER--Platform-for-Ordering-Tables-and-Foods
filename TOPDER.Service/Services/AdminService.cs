using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<Admin> AddAsync(AdminDto adminDto)
        {
            var admin = _mapper.Map<Admin>(adminDto);
            return await _adminRepository.CreateAndReturnAsync(admin);
        }

        public async Task<bool> UpdateAsync(AdminDto adminDto)
        {
            var existingAdmin = await _adminRepository.GetByIdAsync(adminDto.Uid);
            if (existingAdmin == null)
            {
                return false;
            }
            var admin = _mapper.Map<Admin>(adminDto);
            return await _adminRepository.UpdateAsync(admin);
        }

    }
}
