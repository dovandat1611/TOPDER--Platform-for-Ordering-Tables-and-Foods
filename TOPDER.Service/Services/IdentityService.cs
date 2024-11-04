using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.ExternalLogin;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class IdentityService : IIdentityService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly JwtHelper _jwtHelper;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ICustomerService _customerService;
        private readonly IExternalLoginService _externalLoginService;

        public IdentityService(JwtHelper jwtHelper,
            IUserService userService, IWalletService walletService,
            ICustomerService customerService, IExternalLoginService externalLoginService)
        {
            _jwtHelper = jwtHelper;
            _userService = userService;
            _walletService = walletService;
            _customerService = customerService;
            _externalLoginService = externalLoginService;
        }

        public async Task<ApiResponse> AuthenticateWithGoogle(string accessToken)
        {
            try
            {
                // Sử dụng HttpClient đã khai báo ở lớp, không tạo mới mỗi lần gọi
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = errorContent.ToUpper() };
                }

                // Đọc dữ liệu từ phản hồi của Google
                var userInfo = await response.Content.ReadAsStringAsync();
                var user = JObject.Parse(userInfo);

                var userResult = new
                {
                    Id = user["sub"]?.ToString(),
                    Email = user["email"]?.ToString(),
                    Name = user["name"]?.ToString(),
                    Picture = user["picture"]?.ToString()
                };

                if (string.IsNullOrEmpty(userResult.Email))
                {
                    return new ApiResponse { Success = false, Message = "INVALID USER EMAIL" };
                }

                // Kiểm tra xem người dùng đã tồn tại trong hệ thống chưa
                var existingUser = await _userService.GetUserByEmail(userResult.Email);
                if (existingUser != null)
                {
                    var isProfileComplete = await _customerService.CheckProfile(existingUser.Uid);
                    var generatedToken = _jwtHelper.GenerateJwtToken(existingUser);
                    return new ApiResponse
                    {
                        Success = true,
                        Data = generatedToken,
                        Message = "Authentication success"
                    };
                }

                // Tạo người dùng mới nếu chưa tồn tại
                var userDto = new UserDto
                {
                    Email = userResult.Email,
                    RoleId = 3, // CUSTOMER
                    IsVerify = true,
                    Status = Common_Status.ACTIVE,
                    CreatedAt = DateTime.UtcNow,
                    IsExternalLogin = true
                };

                var newUser = await _userService.AddAsync(userDto);
                if (newUser == null)
                {
                    return new ApiResponse { Success = false, Message = "USER CREATION FAILED" };
                }

                // Thêm thông tin đăng nhập ngoại
                var externalLogin = new ExternalLoginDto
                {
                    Id = 0,
                    Uid = newUser.Uid,
                    ExternalProvider = ExternalProvider.GOOGLE,
                    ExternalUserId = userResult.Id ?? Is_Null.ISNULL,
                    AccessToken = accessToken
                };
                await _externalLoginService.AddAsync(externalLogin);

                // Thêm ví
                var walletBalanceDto = new WalletBalanceDto
                {
                    WalletId = 0,
                    Uid = newUser.Uid,
                    WalletBalance = 0
                };
                await _walletService.AddWalletBalanceAsync(walletBalanceDto);

                // Thêm thông tin khách hàng
                var customerRequest = new CreateCustomerRequest
                {
                    Uid = newUser.Uid,
                    Name = userResult.Name,
                    Image = userResult.Picture
                };
                var addedCustomer = await _customerService.AddAsync(customerRequest);

                if (addedCustomer != null)
                {
                    var newUserLoginDto = new UserLoginDTO
                    {
                        Uid = newUser.Uid,
                        Email = newUser.Email,
                        Name = userResult.Name,
                        Role = User_Role.CUSTOMER,
                    };

                    // Tạo JWT token cho người dùng mới
                    var token = _jwtHelper.GenerateJwtToken(newUserLoginDto);
                    return new ApiResponse
                    {
                        Success = true,
                        Data = token,
                        Message = "Authentication success"
                    };
                }

                return new ApiResponse { Success = false, Message = "USER CREATION FAILED" };
            }
            catch (HttpRequestException httpEx)
            {
                return new ApiResponse { Success = false, Message = httpEx.Message.ToUpper() };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message.ToUpper() };
            }
        }
    }
}
