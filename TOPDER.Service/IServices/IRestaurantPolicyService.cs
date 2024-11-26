using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.PolicySystem;
using TOPDER.Service.Dtos.RestaurantPolicy;

namespace TOPDER.Service.IServices
{
    public interface IRestaurantPolicyService
    {
        Task<bool> AddAsync(CreateRestaurantPolicyDto restaurantPolicyDto);
      //  Task<bool> RemoveAsync(int id);
        Task<List<RestaurantPolicyDto>> GetInActivePolicyAsync(int restaurantId);
        Task<bool> ChoosePolicyToUseAsync(int restaurantPolicyId);
        Task<RestaurantPolicyDto> GetActivePolicyAsync(int restaurantId);
    }
}
