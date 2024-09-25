using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Utils;
using TOPDER.Service.Dtos.User;
using TOPDER.Repository.Entities;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ISendMailService _sendMailService;

        public UserController(IRestaurantService restaurantService, CloudinaryService cloudinaryService, ISendMailService sendMailService, IUserService userService, ICustomerService customerService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
            _userService = userService;
            _customerService = customerService;
        }

        [HttpPost("restaurant/register")]
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
                var userDto = new UserDto()
                {
                    Uid = 0,
                    Email = restaurantRequest.Email,
                    RoleId = 2,
                    Password = BCrypt.Net.BCrypt.HashPassword(restaurantRequest.Password),
                    OtpCode = string.Empty,
                    IsVerify = false,
                    Status = Common_Status.INACTIVE,
                    IsExternalLogin = false,
                    CreatedAt = DateTime.Now,
                };
                var user = await _userService.AddAsync(userDto);
                restaurantRequest.Uid = user.Uid; 
                var addedRestaurant = await _restaurantService.AddAsync(restaurantRequest);
                await _sendMailService.SendEmailAsync(user.Email, Email_Subject.VERIFY, EmailTemplates.Verify(addedRestaurant.NameRes, user.Uid));
                return Ok(addedRestaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {ex.Message}");
            }
        }


    }
}
