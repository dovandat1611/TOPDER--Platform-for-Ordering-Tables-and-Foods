using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.RestaurantPolicy;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantPolicyController : ControllerBase
    {
        private readonly IRestaurantPolicyService _restaurantPolicyService;

        public RestaurantPolicyController(IRestaurantPolicyService restaurantPolicyService)
        {
            _restaurantPolicyService = restaurantPolicyService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePolicy([FromBody] CreateRestaurantPolicyDto restaurantPolicyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantPolicyService.AddAsync(restaurantPolicyDto);
            if (result)
                return Ok(new { message = "Restaurant policy created successfully." });
            return StatusCode(500, "An error occurred while creating the restaurant policy.");
        }

        [HttpPut("ChoosePolicyToUse/{id}")]
        public async Task<IActionResult> ChoosePolicyToUse(int id)
        {
            var result = await _restaurantPolicyService.ChoosePolicyToUseAsync(id);
            if (result)
                return Ok(new { message = "Policy status updated successfully." });
            return NotFound("Policy not found or cannot be activated.");
        }

        [HttpGet("GetActivePolicy/{restaurantId}")]
        public async Task<IActionResult> GetActivePolicy(int restaurantId)
        {
            var policy = await _restaurantPolicyService.GetActivePolicyAsync(restaurantId);
            if (policy == null)
                return NotFound("No active policy found for this restaurant.");
            return Ok(policy);
        }

        [HttpGet("GetInActivePolicies/{restaurantId}")]
        public async Task<IActionResult> GetInActivePolicies(int restaurantId)
        {
            var policies = await _restaurantPolicyService.GetInActivePolicyAsync(restaurantId);
            if (policies == null || policies.Count == 0)
                return NotFound("No inactive policies found for this restaurant.");
            return Ok(policies);
        }

    }
}
