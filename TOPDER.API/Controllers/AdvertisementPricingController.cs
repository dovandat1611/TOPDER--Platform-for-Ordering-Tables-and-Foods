using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementPricingController : ControllerBase
    {
        private readonly IAdvertisementPricingService _advertisementPricingService;
        private readonly IRestaurantRepository _restaurantRepository;

        public AdvertisementPricingController(IAdvertisementPricingService advertisementPricingService, IRestaurantRepository restaurantRepository)
        {
            _advertisementPricingService = advertisementPricingService;
            _restaurantRepository = restaurantRepository;
        }

        [HttpGet("FixAll15minus")]
        public async Task<IActionResult> FixAll15minus()
        {
            var advertisementPricings = await _restaurantRepository.GetAllAsync();

            foreach(var item in  advertisementPricings)
            {
                item.Subdescription = "https://viettinlaw.com/wp-content/uploads/2013/07/gcndkkd.jpg";
                await _restaurantRepository.UpdateAsync(item);
            }
            return Ok();
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
