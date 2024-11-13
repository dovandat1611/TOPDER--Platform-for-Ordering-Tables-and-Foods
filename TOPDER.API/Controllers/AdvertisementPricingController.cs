using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementPricingController : ControllerBase
    {
        private readonly IAdvertisementPricingService _advertisementPricingService;

        public AdvertisementPricingController(IAdvertisementPricingService advertisementPricingService)
        {
            _advertisementPricingService = advertisementPricingService;
        }

        [HttpGet("GetAllAdvertisementPricing")]
        public async Task<IActionResult> GetAll()
        {
            var advertisementPricings = await _advertisementPricingService.GetAllAdvertisementPricingAsync();
            return Ok(advertisementPricings);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AdvertisementPricingDto advertisementPricingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _advertisementPricingService.AddAsync(advertisementPricingDto);
            if (!result)
                return BadRequest("Failed to create Advertisement Pricing.");

            return Ok("Advertisement Pricing created successfully.");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] AdvertisementPricingDto advertisementPricingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _advertisementPricingService.UpdateAsync(advertisementPricingDto);
            if (!result)
                return NotFound("Advertisement Pricing not found.");

            return Ok("Advertisement Pricing updated successfully.");
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _advertisementPricingService.RemoveAsync(id);
            if (!result)
                return NotFound("Advertisement Pricing not found.");

            return Ok("Advertisement Pricing deleted successfully.");
        }
    }
}
