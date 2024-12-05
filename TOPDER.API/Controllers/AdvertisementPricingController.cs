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



/*
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] AdvertisementPricingDto advertisementPricingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _advertisementPricingService.UpdateAsync(advertisementPricingDto);
            if (!result)
                return NotFound("Advertisement Pricing not found.");

            return Ok("Advertisement Pricing updated successfully.");
        }*/

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
