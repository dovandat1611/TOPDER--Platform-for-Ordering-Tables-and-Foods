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
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Services;
using Org.BouncyCastle.Asn1.Ocsp;

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
        private readonly IAdminService _adminService;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IUserOtpRepository _userOtpRepository;
        private readonly IRoleRepository _roleRepository;




        public UserController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService,
            ISendMailService sendMailService, IUserService userService, ICustomerService customerService,
            IWalletService walletService, JwtHelper jwtHelper, IAdminService adminService,
            IUserRepository userRepository, IIdentityService identityService,
            IUserOtpRepository userOtpRepository, IRoleRepository roleRepository)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
            _userService = userService;
            _customerService = customerService;
            _walletService = walletService;
            _jwtHelper = jwtHelper;
            _userRepository = userRepository;
            _identityService = identityService;
            _adminService = adminService;
            _userOtpRepository = userOtpRepository;
            _roleRepository = roleRepository;
        }


        [HttpGet("GetProfile/{uid}")]
        [SwaggerOperation(Summary = "Lấy thông tin profile của Current User")]
        public async Task<IActionResult> GetProfile(int uid)
        {
            var role = await _userService.GetRoleUserProfile(uid);

            if (role.Role.Equals(User_Role.CUSTOMER))
            {
                var profile = await _customerService.Profile(uid);
                profile.WalletBalance = role.WalletBalance;
                profile.Role = role.Role;
                if (profile == null)
                    return NotFound(new { Message = "Không tìm thấy khách hàng." });

                return Ok(profile);
            }

            if (role.Role.Equals(User_Role.RESTAURANT))
            {
                var profile = await _restaurantService.Profile(uid);

                profile.WalletBalance = role.WalletBalance;
                profile.Role = role.Role;
                if (profile == null)
                    return NotFound(new { Message = "Không tìm thấy nhà hàng." });

                return Ok(profile);
            }

            if (role.Role.Equals(User_Role.ADMIN))
            {
                var profile = await _adminService.Profile(uid);
                profile.Role = role.Role;

                if (profile == null)
                    return NotFound(new { Message = "Không tìm thấy admin." });

                return Ok(profile);
            }

            return NotFound(new { Message = "Không tìm thấy user hoặc user không hợp lệ." });
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
                object userInfo = null;
                if (userLoginDto.Role.Equals(User_Role.RESTAURANT))
                {
                    RestaurantInfoResponse restaurantInfo = new RestaurantInfoResponse()
                    {
                        Uid = userLoginDto.Uid,
                        Email = userLoginDto.Email,
                        CategoryRestaurantId = userLoginDto.CategoryRestaurantId,
                        CategoryRestaurantName = userLoginDto.CategoryRestaurantName,
                        NameOwner = userLoginDto.NameOwner,
                        NameRes = userLoginDto.NameRes,
                        Phone = userLoginDto.Phone,
                        Logo = userLoginDto.Logo,
                        OpenTime = userLoginDto.OpenTime,
                        CloseTime = userLoginDto.CloseTime,
                        Address = userLoginDto.Address,
                        Description = userLoginDto.Description,
                        Subdescription = userLoginDto.Subdescription,
                        ProvinceCity = userLoginDto.ProvinceCity,
                        District = userLoginDto.District,
                        Commune = userLoginDto.Commune,
                        Discount = userLoginDto.Discount,
                        MaxCapacity = userLoginDto.MaxCapacity,
                        Price = userLoginDto.Price,
                        IsBookingEnabled = userLoginDto.IsBookingEnabled,
                        Role = userLoginDto.Role
                    };
                    userInfo = restaurantInfo;
                }
                if (userLoginDto.Role.Equals(User_Role.ADMIN))
                {
                    AdminInfoRespone adminInfo = new AdminInfoRespone()
                    {
                        Uid = userLoginDto.Uid,
                        Email = userLoginDto.Email,
                        Name = userLoginDto.Name,
                        Phone = userLoginDto.Phone,
                        Image = userLoginDto.Image,
                        Dob = userLoginDto.Dob,
                        Role = userLoginDto.Role
                    };
                    userInfo = adminInfo;
                }
                if (userLoginDto.Role.Equals(User_Role.CUSTOMER))
                {
                    CustomerInfoResponse customerInfo = new CustomerInfoResponse()
                    {
                        Uid = userLoginDto.Uid,
                        Email = userLoginDto.Email,
                        Name = userLoginDto.Name,
                        Phone = userLoginDto.Phone,
                        Image = userLoginDto.Image,
                        Dob = userLoginDto.Dob,
                        Gender = userLoginDto.Gender,
                        Role = userLoginDto.Role
                    };
                    userInfo = customerInfo;
                }
                if (userInfo == null)
                {
                    return BadRequest(new { message = "Role không hợp lệ." });
                }

                var token = _jwtHelper.GenerateJwtToken(userLoginDto);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Authentication success.",
                    Data = new
                    {
                        Token = token,
                        UserInfo = userInfo
                    },
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
        public async Task<IActionResult> CheckAccessToken(string accessToken)
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

            bool exists = await _userService.CheckExistEmail(restaurantRequest.Email);
            if (exists)
            {
                return BadRequest(new { message = "Email đã tồn tại trong hệ thống vui lòng thử một email khác." });
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

            bool exists = await _userService.CheckExistEmail(customerRequest.Email);
            if (exists)
            {
                return BadRequest(new { message = "Email đã tồn tại trong hệ thống vui lòng thử một email khác ." });
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
                AdminDto admin = new AdminDto()
                {
                    Uid = user.Uid,
                    Name = "TOPDER",
                    Phone = "0968519615",
                    Dob = DateTime.Now,
                    Image = Default_Avatar.ADMIN
                };

                var addedAdmin = await _adminService.AddAsync(admin);

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

            user.Status = status;

            var update = await _userRepository.UpdateAsync(user);

            if (update)
            {   
                if(status == Common_Status.ACTIVE)
                {
                    var role = await _roleRepository.GetByIdAsync(user.RoleId);
                    if (role.Name == User_Role.RESTAURANT)
                    {
                        await _sendMailService.SendEmailAsync(user.Email, Email_Subject.CONFIRM_REGISTER_RESTAURANT, EmailTemplates.ConfirmRegisterRestaurant());
                    }
                }
                return Ok("Cập nhật trạng thái người dùng thành công.");
            }

            return BadRequest("Cập nhật trạng thái người dùng thất bại.");
        }

        [HttpPost("ForgotPassword")]
        [SwaggerOperation(Summary = "Quên mật khẩu - OTP - Đặt lại mật khẩu")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
                return BadRequest($"Không tìm thấy tài khoản với email: {email}");

            var otpCode = new Random().Next(100000, 999999).ToString();
            var expiresAt = DateTime.Now.AddMinutes(5); 

            var userOtp = new UserOtp
            {
                Uid = user.Uid,
                OtpCode = otpCode,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.Now,
                IsUse = false,
            };

            var createOTP = await _userOtpRepository.CreateAsync(userOtp);
            if (!createOTP)
                return BadRequest("Không thể tạo OTP");

            var subject = Email_Subject.OTP;
            var body = user.Role.Equals(User_Role.RESTAURANT)
                ? EmailTemplates.OTP(user.NameRes, otpCode)
                : EmailTemplates.OTP(user.Name, otpCode);

            await _sendMailService.SendEmailAsync(email, subject, body);
            return Ok("OTP đã được gửi đến email của bạn.");
        }

        [HttpPost("VerifyOTP")]
        [SwaggerOperation(Summary = "Quên mật khẩu - OTP - Đặt lại mật khẩu")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOtpRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);

            if (user == null)
                return BadRequest($"Không tìm thấy tài khoản với email: {request.Email}");

            var getOTP = await _userOtpRepository.GetValidOtpAsync(user.Uid, request.Otp);

            if (getOTP == null)
                return BadRequest("Không thấy OTP nào hợp lệ hoặc OTP đã hết hạn. Hãy thử tạo lại OTP (Thời gian hiệu lực là 5 phút).");

            if (string.Equals(request.Otp, getOTP.OtpCode, StringComparison.OrdinalIgnoreCase))
            {
                getOTP.IsUse = true;
                var success = await _userOtpRepository.UpdateAsync(getOTP);
                if (success)
                    return Ok("OTP đã được xác thực thành công.");
                return StatusCode(500, "Đã xảy ra lỗi khi đánh dấu OTP là đã sử dụng.");
            }
            return BadRequest("OTP không đúng.");
        }

        [HttpPost("ResetPassword")]
        [SwaggerOperation(Summary = "Quên mật khẩu - OTP - Đặt lại mật khẩu")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var query = await _userRepository.QueryableAsync();

            var user = query.FirstOrDefault(x => x.Email.Equals(request.Email));

            if (user == null)
                return BadRequest($"Không tìm thấy tài khoản với email: {request.Email}");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword); // Hash password

            var result = await _userRepository.UpdateAsync(user);

            if (result)
                return Ok("Mật khẩu đã được đặt lại thành công.");

            return StatusCode(500, "Lỗi khi cập nhật mật khẩu.");
        }

        [HttpPost("ChangePassword")]
        [SwaggerOperation(Summary = "Đổi mật khẩu")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await _userService.ChangePassword(request);

            if (!result)
                return BadRequest("Mật khẩu cũ không đúng hoặc không tìm thấy tài khoản.");

            return Ok("Mật khẩu đã được thay đổi thành công.");
        }


        [HttpGet("CheckExistEmail")]
        [SwaggerOperation(Summary = "Khi người dùng ngừng nhập ô email được 1s thì sẽ check xem email đã tồn tại hay chưa và trả về cho họ thông báo")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool exists = await _userService.CheckExistEmail(email);
            if (exists)
            {
                return Ok(new { message = "Email đã tồn tại trong hệ thống vui lòng thử một email khác ." });
            }
            return NotFound(new { message = "Email chưa được đăng ký trong hệ thống và hợp lệ." });
        }


        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
        }


    }
}
