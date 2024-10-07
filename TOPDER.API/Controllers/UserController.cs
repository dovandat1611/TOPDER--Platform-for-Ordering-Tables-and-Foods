using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Utils;
using TOPDER.Service.Dtos.User;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Repository.Repositories;
using TOPDER.Repository.IRepositories;

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
        private readonly JwtHelper _jwtHelper;
        private readonly AdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;



        public UserController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService,
            ISendMailService sendMailService, IUserService userService, ICustomerService customerService,
            IWalletService walletService, JwtHelper jwtHelper, AdminRepository adminRepository, IUserRepository userRepository)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
            _userService = userService;
            _customerService = customerService;
            _walletService = walletService;
            _jwtHelper = jwtHelper;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var userLoginDto = await _userService.GetUserByEmailAndPassword(loginModel);
                var token = _jwtHelper.GenerateJwtToken(userLoginDto);
                return Ok(new
                {
                    user = userLoginDto,
                    token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra. Vui lòng thử lại sau.", error = ex.Message });
            }
        }


        [HttpPost("restaurant/register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterRestaurant([FromForm] CreateRestaurantRequest restaurantRequest)
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
                    RoleId = 2, // Restaurant
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


        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer([FromForm] CreateCustomerRequest customerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userDto = new UserDto()
                {
                    Uid = 0,
                    Email = customerRequest.Email,
                    RoleId = 3, // CUSTOMER
                    Password = BCrypt.Net.BCrypt.HashPassword(customerRequest.Password),
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
                customerRequest.Uid = user.Uid;
                customerRequest.Image = Default_Avatar.CUSTOMER;
                var addedCustomer = await _customerService.AddAsync(customerRequest);

                // SEND EMAIL
                await _sendMailService.SendEmailAsync(user.Email, Email_Subject.VERIFY, EmailTemplates.Verify(customerRequest.Name, user.Uid));

                return Ok(addedCustomer);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {errorMessage}");
            }
        }


        [HttpPost("admin/register")]
        public async Task<IActionResult> RegisterAdmin()
        {
            try
            {
                var userDto = new UserDto()
                {
                    Uid = 0,
                    Email = "topder.vn@gmail.com",
                    RoleId = 1, // Admin
                    Password = BCrypt.Net.BCrypt.HashPassword("Topder123"),
                    OtpCode = string.Empty,
                    IsVerify = true,
                    Status = Common_Status.ACTIVE,
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
                Admin admin = new Admin()
                {
                    Uid = user.Uid,
                    Name = "TOPDER",
                    Phone = "0968519615",
                    Dob = DateTime.Now,
                    Image = Default_Avatar.ADMIN
                };
                var addedAdmin = await _adminRepository.CreateAndReturnAsync(admin);

                // SEND EMAIL
                await _sendMailService.SendEmailAsync(user.Email, Email_Subject.VERIFY, EmailTemplates.Verify(admin.Name, user.Uid));

                return Ok(addedAdmin);
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

        [HttpGet("updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {   

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            if (string.IsNullOrEmpty(status) ||
                (!status.Equals(Common_Status.INACTIVE) && !status.Equals(Common_Status.ACTIVE)))
            {
                return BadRequest("Trạng thái không hợp lệ. Vui lòng chọn ACTIVE hoặc INACTIVE.");
            }


            if (status.Equals(user.Status))
            {
                return Ok("Người dùng đã có trạng thái này.");
            }

            var update = await _userRepository.ChangeStatusAsync(id, status);
            if (update)
            {
                return Ok("Cập nhật trạng thái người dùng thành công.");
            }

            return BadRequest("Cập nhật trạng thái người dùng thất bại.");
        }


    }
}
