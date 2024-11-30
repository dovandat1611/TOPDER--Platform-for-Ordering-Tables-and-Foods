using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.PolicySystem;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class PolicySystemService : IPolicySystemService
    {
        private readonly IMapper _mapper;
        private readonly IPolicySystemRepository _policySystemRepository;

        public PolicySystemService(IPolicySystemRepository policySystemRepository, IMapper mapper)
        {
            _mapper = mapper;
            _policySystemRepository = policySystemRepository;
        }

        public async Task<bool> AddAsync(CreatePolicySystemDto policySystemDto)
        {
            var policySystem = _mapper.Map<PolicySystem>(policySystemDto);
            policySystem.PolicyId = 0;
            policySystem.CreateDate = DateTime.Now;
            policySystem.Status = Common_Status.ACTIVE;
            return await _policySystemRepository.CreateAsync(policySystem);
        }

        public async Task<List<PolicySystemDto>> GetAllAsync()
        {
            var policySystems = await _policySystemRepository.GetAllAsync();

            var policySystemDtos = _mapper.Map<List<PolicySystemDto>>(policySystems);

            return policySystemDtos;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var policySystem = await _policySystemRepository.GetByIdAsync(id);
            if (policySystem == null)
            {
                return false;
            }
            var result = await _policySystemRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> UpdateAsync(UpdatePolicySystemDto policySystemDto)
        {
            var existingPolicySystem = await _policySystemRepository.GetByIdAsync(policySystemDto.PolicyId);
            if (existingPolicySystem == null)
            {
                return false;
            }
            existingPolicySystem.MinOrderValue = policySystemDto.MinOrderValue;
            if(policySystemDto.MaxOrderValue > 0)
            {
                existingPolicySystem.MaxOrderValue = policySystemDto.MaxOrderValue;
            }
            existingPolicySystem.FeeAmount = policySystemDto.FeeAmount;
            existingPolicySystem.Status = policySystemDto.Status;
            return await _policySystemRepository.UpdateAsync(existingPolicySystem);
        }
    }
}
