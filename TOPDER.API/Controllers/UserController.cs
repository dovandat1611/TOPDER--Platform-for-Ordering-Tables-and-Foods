﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Utils;
using TOPDER.Service.Dtos.User;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Wallet;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IWalletService _walletService;
        private readonly ISendMailService _sendMailService;

        public UserController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService, ISendMailService sendMailService, IUserService userService, ICustomerService customerService, IWalletService walletService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
            _userService = userService;
            _customerService = customerService;
            _walletService = walletService;
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

            restaurantRequest.Logo = uploadResult.SecureUrl?.ToString();

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

                //ADD USER
                var user = await _userService.AddAsync(userDto);

                // ADD WALLET
                WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
                {
                    WalletId = 0,
                    Uid = user.Uid,
                    WalletBalance = 0
                };
                
                var wallet = await _walletService.AddWalletBalanceAsync(walletBalanceDto);
                
                // ADD RESTAURANT
                restaurantRequest.Uid = user.Uid; 
                var addedRestaurant = await _restaurantService.AddAsync(restaurantRequest);
                
                // SEND EMAIL
                await _sendMailService.SendEmailAsync(user.Email, Email_Subject.VERIFY, EmailTemplates.Verify(addedRestaurant.NameRes, user.Uid));
                
                return Ok(addedRestaurant);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {errorMessage}");
            }
        }


        [HttpGet("verify/{id}")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            var isVerified = await _userService.Verify(id);
            if (!isVerified)
            {
                return NotFound("User not found or already verified.");
            }
            return Ok("User successfully verified.");
        }

    }
}
