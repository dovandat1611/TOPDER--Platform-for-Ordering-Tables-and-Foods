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
using TOPDER.Service.Dtos.RestaurantPolicy;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class RestaurantPolicyService : IRestaurantPolicyService
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantPolicyRepository _restaurantPolicyRepository;

        public RestaurantPolicyService(IRestaurantPolicyRepository restaurantPolicyRepository, IMapper mapper)
        {
            _mapper = mapper;
            _restaurantPolicyRepository = restaurantPolicyRepository;
        }

        public async Task<bool> AddAsync(CreateRestaurantPolicyDto restaurantPolicyDto)
        {
            var restaurantPolicy = _mapper.Map<RestaurantPolicy>(restaurantPolicyDto);
            restaurantPolicy.PolicyId = 0;
            restaurantPolicy.CreateDate = DateTime.Now;
            restaurantPolicy.Status = Common_Status.INACTIVE;
            return await _restaurantPolicyRepository.CreateAsync(restaurantPolicy);
        }

        public async Task<bool> ChoosePolicyToUseAsync(int restaurantPolicyId)
        {
            var query = await _restaurantPolicyRepository.QueryableAsync();

            var policyToUse = await query.FirstOrDefaultAsync(x => x.PolicyId == restaurantPolicyId);

            var policyActive = await query.Where(x => x.Status == Common_Status.ACTIVE).ToListAsync();

            if (policyToUse == null)
            {
                return false;
            }

            policyToUse.Status = Common_Status.ACTIVE;

            if(policyActive != null)
            {
                foreach(var policy in policyActive)
                {
                    policy.Status = Common_Status.INACTIVE;
                    await _restaurantPolicyRepository.UpdateAsync(policy);
                }
            }

            var result = await _restaurantPolicyRepository.UpdateAsync(policyToUse);

            return result;
        }

        public async Task<RestaurantPolicyDto> GetActivePolicyAsync(int restaurantId)
        {
            var query = await _restaurantPolicyRepository.QueryableAsync();
            var policyActive = await query.FirstOrDefaultAsync(x => x.Status == Common_Status.ACTIVE && x.RestaurantId == restaurantId);

            var policyActiveDto = _mapper.Map<RestaurantPolicyDto>(policyActive);

            return policyActiveDto;
        }

        public async Task<List<RestaurantPolicyDto>> GetInActivePolicyAsync(int restaurantId)
        {
            var query = await _restaurantPolicyRepository.QueryableAsync();
            var policyInActive = await query.Where(x => x.Status == Common_Status.INACTIVE)
                .OrderByDescending(x => x.PolicyId).ToListAsync();

            var policyInActiveDto = _mapper.Map<List<RestaurantPolicyDto>>(policyInActive);

            return policyInActiveDto;
        }
    }
}
