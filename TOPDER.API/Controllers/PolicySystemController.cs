using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.PolicySystem;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicySystemController : ControllerBase
    {
        private readonly IPolicySystemService _policySystemService;

        public PolicySystemController(IPolicySystemService policySystemService)
        {
            _policySystemService = policySystemService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicySystemDto policySystemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policySystemService.AddAsync(policySystemDto);
            if (result)
                return Ok(new { message = "Policy created successfully." });
            return StatusCode(500, "An error occurred while creating the policy.");
        }

        [HttpGet("GetAllPolicies")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var policies = await _policySystemService.GetAllAsync();
            return Ok(policies);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePolicy(int id)
        {
            var result = await _policySystemService.RemoveAsync(id);
            if (result)
                return Ok(new { message = "Policy deleted successfully." });
            return NotFound("Policy not found.");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdatePolicy([FromBody] UpdatePolicySystemDto policySystemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policySystemService.UpdateAsync(policySystemDto);
            if (result)
                return Ok(new { message = "Policy updated successfully." });
            return NotFound("Policy not found.");
        }
    }
}
