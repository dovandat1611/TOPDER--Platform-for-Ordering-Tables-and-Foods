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
using Swashbuckle.AspNetCore.Annotations;

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
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;



        public UserController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService,
            ISendMailService sendMailService, IUserService userService, ICustomerService customerService,
            IWalletService walletService, JwtHelper jwtHelper, IAdminRepository adminRepository,
            IUserRepository userRepository, IIdentityService identityService)
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
            _identityService = identityService;
        }

        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Login bằng tài khoản và mật khẩu")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var userLoginDto = await _userService.GetUserByEmailAndPassword(loginModel);
                var isProfileComplete = true;
                if (userLoginDto.RoleName.Equals(User_Role.CUSTOMER))
                {
                    isProfileComplete = await _customerService.CheckProfile(userLoginDto.Uid);
                }
                var token = _jwtHelper.GenerateJwtToken(userLoginDto);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Authentication success.",
                    Data = new
                    {
                        Token = token
                    },
                    IsProfileComplete = isProfileComplete
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

        [HttpPost("LoginWithGoogle")]
        [SwaggerOperation(Summary = "Login bằng google | Nếu chưa có tài khoản trong hệ thống thì sẽ tạo mới(Giống flow của RegisterCustomer)")]
        public async Task<IActionResult> CheckAccessToken([FromBody] string accessToken)
        {
            var result = await _identityService.AuthenticateWithGoogle(accessToken);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }


        [HttpPost("RegisterRestaurant")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "(Tạo User) sau đó (Tạo Wallet) cho User sau đó (Tạo Restaurant) và (SendEmail VerifyAccount)")]
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

                if(user != null)
                {
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

                    if (wallet == false)
                    {
                        return BadRequest("Không tạo được Ví cho người dùng.");
                    }

                    return Ok(addedRestaurant);
                }
                return BadRequest("Tạo người dùng thất bại.");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {errorMessage}");
            }
        }


        [HttpPost("RegisterCustomer")]
        [SwaggerOperation(Summary = "(Tạo User) sau đó (Tạo Wallet) cho User sau đó (Tạo Customer) và (SendEmail VerifyAccount)")]
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
                    Status = Common_Status.ACTIVE,
                    IsExternalLogin = false,
                    CreatedAt = DateTime.Now,
                };

                //ADD USER
                var user = await _userService.AddAsync(userDto);
                if(user != null)
                {
                    // ADD WALLET
                    WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
                    {
                        WalletId = 0,
                        Uid = user.Uid,
                        WalletBalance = 0
                    };

                    var wallet = await _walletService.AddWalletBalanceAsync(walletBalanceDto);

                    // ADD Customer
                    customerRequest.Uid = user.Uid;
                    customerRequest.Image = Default_Avatar.CUSTOMER;
                    var addedCustomer = await _customerService.AddAsync(customerRequest);

                    // SEND EMAIL
                    await _sendMailService.SendEmailAsync(user.Email, Email_Subject.VERIFY, EmailTemplates.Verify(customerRequest.Name ?? Is_Null.ISNULL, user.Uid));

                    if(wallet == false)
                    {
                        return BadRequest("Không tạo được Ví cho người dùng.");
                    }
                    return Ok(addedCustomer);
                }
                return BadRequest("Tạo người dùng thất bại.");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant: {errorMessage}");
            }
        }


        [HttpPost("RegisterAdmin")]
        [SwaggerOperation(Summary = "Tạo Admin: Lưu ý! chỉ dùng 1 lần khi chưa có data của Admin")]
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

        [HttpGet("VerifyAccount/{uid}")]
        [SwaggerOperation(Summary = "Cập nhật lại trường thông tin có tên IsVerify trong User thành TRUE")]
        public async Task<IActionResult> VerifyUser(int uid)
        {
            var isVerified = await _userService.Verify(uid);
            if (!isVerified)
            {
                return NotFound("User not found or already verified.");
            }
            return Ok("User successfully verified.");
        }

        [HttpGet("UpdateStatus/{uid}")]
        [SwaggerOperation(Summary = "Cập nhật lại trạng thái của người dùng (In-Active, Active)")]
        public async Task<IActionResult> UpdateStatus(int uid, string status)
        {   

            var user = await _userRepository.GetByIdAsync(uid);
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

            var update = await _userRepository.ChangeStatusAsync(uid, status);
            if (update)
            {
                return Ok("Cập nhật trạng thái người dùng thành công.");
            }

            return BadRequest("Cập nhật trạng thái người dùng thất bại.");
        }


    }
}
