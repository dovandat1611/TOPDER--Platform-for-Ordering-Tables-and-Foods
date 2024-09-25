using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ISendMailService _sendMailService;

        public RestaurantController(IRestaurantService restaurantService, CloudinaryService cloudinaryService, ISendMailService sendMailService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
        }

        //[HttpGet("restaurant-home")]
        //public async Task<IActionResult> RestaurantHome(int pageNumber, int pageSize)
        //{
        //    var paginatedDTOs = await _restaurantService.GetItemsAsync(pageNumber, pageSize);

        //    var response = new PaginatedResponseDto<RestaurantHomeDto>(
        //        paginatedDTOs,
        //        paginatedDTOs.PageIndex,
        //        paginatedDTOs.TotalPages,
        //        paginatedDTOs.HasPreviousPage,
        //        paginatedDTOs.HasNextPage
        //    );
        //    return Ok(response);
        //}


        [HttpGet("send-mail")]
        public async Task<IActionResult> SendMail(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                return BadRequest("To, Subject, and Body are required.");
            }
            try
            {
                await _sendMailService.SendEmailAsync(to, subject, body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("register-restaurant")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] CreateRestaurantRequest restaurantRequest)
        {
            if (restaurantRequest.File == null || restaurantRequest.File.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var uploadResult = await _cloudinaryService.UploadImageAsync(restaurantRequest.File);

            if (uploadResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Image upload failed.");
            }

            restaurantRequest.Image = uploadResult.SecureUrl?.ToString();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var addedRestaurant = await _restaurantService.AddAsync(restaurantRequest);
                return Ok(addedRestaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {ex.Message}");
            }
        }


    }
}
