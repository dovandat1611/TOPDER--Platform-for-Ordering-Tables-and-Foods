using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
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
        private readonly JwtHelper _jwtHelper;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ICustomerService _customerService;
        private readonly IExternalLoginService _externalLoginService;
        private readonly string _clientId;


        public IdentityService(JwtHelper jwtHelper,
            IUserService userService, IWalletService walletService,
            ICustomerService customerService,
            IExternalLoginService externalLoginService, IConfiguration configuration)
        {
            _jwtHelper = jwtHelper;
            _userService = userService;
            _walletService = walletService;
            _customerService = customerService;
            _externalLoginService = externalLoginService;
            _clientId = configuration["Authentication:Google:ClientId"];
        }

        public async Task<Repository.Entities.ApiResponse> AuthenticateWithGoogle(string accessToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId } // Đảm bảo rằng client ID khớp
                });

                // Nếu xác thực thành công, trả về thông tin người dùng
                UserInfoLoginGoole userInfoLogin = new UserInfoLoginGoole()
                {
                    Id = payload.JwtId,
                    Email = payload.Email,
                    Name = payload.Name,
                    Picture = payload.Picture
                };

                if (string.IsNullOrEmpty(userInfoLogin.Email))
                {
                    return new Repository.Entities.ApiResponse { Success = false, Message = "INVALID USER EMAIL" };
                }

                // Kiểm tra xem người dùng đã tồn tại trong hệ thống chưa
                var existingUser = await _userService.GetUserByEmail(userInfoLogin.Email);
                if (existingUser != null)
                {
                    var isProfileComplete = await _customerService.CheckProfile(existingUser.Uid);
                    var generatedToken = _jwtHelper.GenerateJwtToken(existingUser);
                    return new Repository.Entities.ApiResponse
                    {
                        Success = true,
                        Data = generatedToken,
                        Message = "Authentication success"
                    };
                }

                // Tạo người dùng mới nếu chưa tồn tại
                var userDto = new UserDto
                {
                    Email = userInfoLogin.Email,
                    RoleId = 3, // CUSTOMER
                    IsVerify = true,
                    Status = Common_Status.ACTIVE,
                    CreatedAt = DateTime.UtcNow,
                    IsExternalLogin = true
                };

                var newUser = await _userService.AddAsync(userDto);
                if (newUser == null)
                {
                    return new Repository.Entities.ApiResponse { Success = false, Message = "USER CREATION FAILED" };
                }

                // Thêm thông tin đăng nhập ngoại
                var externalLogin = new ExternalLoginDto
                {
                    Id = 0,
                    Uid = newUser.Uid,
                    ExternalProvider = ExternalProvider.GOOGLE,
                    ExternalUserId = userInfoLogin.Id ?? Is_Null.ISNULL,
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
                    Name = userInfoLogin.Name,
                    Image = userInfoLogin.Picture
                };
                var addedCustomer = await _customerService.AddAsync(customerRequest);

                if (addedCustomer != null)
                {
                    var newUserLoginDto = new UserLoginDTO
                    {
                        Uid = newUser.Uid,
                        Email = newUser.Email,
                        Name = userInfoLogin.Name,
                        Role = User_Role.CUSTOMER,
                    };

                    // Tạo JWT token cho người dùng mới
                    var token = _jwtHelper.GenerateJwtToken(newUserLoginDto);
                    return new Repository.Entities.ApiResponse
                    {
                        Success = true,
                        Data = token,
                        Message = "Authentication success"
                    };
                }

                return new Repository.Entities.ApiResponse { Success = false, Message = "USER CREATION FAILED" };
            }
            catch (HttpRequestException httpEx)
            {
                return new Repository.Entities.ApiResponse { Success = false, Message = httpEx.Message.ToUpper() };
            }
            catch (Exception ex)
            {
                return new Repository.Entities.ApiResponse { Success = false, Message = ex.Message.ToUpper() };
            }
        }
    }
}