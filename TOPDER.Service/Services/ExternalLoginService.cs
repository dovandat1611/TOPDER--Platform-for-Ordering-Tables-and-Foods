using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.ExternalLogin;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IMapper _mapper;
        private readonly IExternalLoginRepository _externalLoginRepository;

        public ExternalLoginService(IExternalLoginRepository externalLoginRepository, IMapper mapper)
        {
            _externalLoginRepository = externalLoginRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(ExternalLoginDto externalLoginDto)
        {
            var externalLogin = _mapper.Map<ExternalLogin>(externalLoginDto);
            return await _externalLoginRepository.CreateAsync(externalLogin);
        }

        public async Task<bool> UpdateAsync(ExternalLoginDto externalLoginDto)
        {
            var existingExternalLogin = await _externalLoginRepository.GetByIdAsync(externalLoginDto.Id);
            if (existingExternalLogin == null || externalLoginDto.Id != existingExternalLogin.Id)
            {
                return false;
            }
            var externalLogin = _mapper.Map<ExternalLogin>(externalLoginDto);
            return await _externalLoginRepository.UpdateAsync(externalLogin);
        }
    }
}
