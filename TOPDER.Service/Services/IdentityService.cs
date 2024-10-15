using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        private readonly HttpClient _httpClient;
        private readonly JwtHelper _jwtHelper;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ICustomerService _customerService;
        private readonly IExternalLoginService _externalLoginService;



        public IdentityService(HttpClient httpClient, JwtHelper jwtHelper,
            IUserService userService, IWalletService walletService,
            ICustomerService customerService, IExternalLoginService externalLoginService)
        {
            _httpClient = httpClient;
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
                // Validate token and retrieve user info from Google API in one call
                var userInfoUrl = $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={accessToken}";
                var userInfoResponse = await _httpClient.GetAsync(userInfoUrl);

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    var userInfoError = await userInfoResponse.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = userInfoError.ToUpper() };
                }

                var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
                var user = JObject.Parse(userInfo);

                var userResult = new
                {
                    Id = user["id"]?.ToString(),
                    Email = user["email"]?.ToString(),
                    Name = user["name"]?.ToString(),
                    Picture = user["picture"]?.ToString()
                };

                if (string.IsNullOrEmpty(userResult.Email))
                {
                    return new ApiResponse { Success = false, Message = "INVALID USER EMAIL" };
                }

                // Check if the user already exists in the system
                var existingUser = await _userService.GetUserByEmail(userResult.Email);
                if (existingUser != null)
                {
                    // Existing user, generate token
                    var isProfileComplete = await _customerService.CheckProfile(existingUser.Uid);
                    var generatedToken = _jwtHelper.GenerateJwtToken(existingUser);
                    return new ApiResponse
                    {
                        Success = true,
                        Data = generatedToken,
                        Message = "Authentication success",
                        IsProfileComplete = isProfileComplete,
                    };
                }

                // Create a new user if they don't exist
                var userDto = new UserDto
                {
                    Email = userResult.Email,
                    RoleId = 3, // CUSTOMER
                    IsVerify = true,
                    Status = Common_Status.ACTIVE,
                    CreatedAt = DateTime.UtcNow,
                    IsExternalLogin = true
                };

                //ADD USER
                var newUser = await _userService.AddAsync(userDto);

                if (newUser != null)
                {
                    //ADD ExternalLogin 
                    var externalLogin = new ExternalLoginDto
                    {
                        Id = 0,
                        Uid = newUser.Uid,
                        ExternalProvider = ExternalProvider.GOOGLE,
                        ExternalUserId = userResult.Id ?? Is_Null.ISNULL,
                        AccessToken = accessToken
                    };

                    await _externalLoginService.AddAsync(externalLogin);

                    // ADD WALLET
                    WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
                    {
                        WalletId = 0,
                        Uid = newUser.Uid,
                        WalletBalance = 0
                    };

                    var wallet = await _walletService.AddWalletBalanceAsync(walletBalanceDto);

                    // ADD CUSTOMER
                    CreateCustomerRequest customerRequest = new CreateCustomerRequest()
                    {
                        Uid = newUser.Uid,
                        Name = userResult.Name,
                        Image = userResult.Picture,
                    };

                    var addedCustomer = await _customerService.AddAsync(customerRequest);

                    if(addedCustomer != null)
                    {
                        var newUserLoginDto = new UserLoginDTO() 
                        {
                            Uid = newUser.Uid,
                            Email = newUser.Email,
                            Name = userResult.Name,
                            RoleName = User_Role.CUSTOMER,
                        };

                        // Generate token for the newly created user
                        var token = _jwtHelper.GenerateJwtToken(newUserLoginDto);
                        return new ApiResponse
                        {
                            Success = true,
                            Data = token,
                            Message = "Authentication success",
                            IsProfileComplete = false
                        };
                    }
                }
                return new ApiResponse { Success = false, Message = "USER CREATION FAILED" };
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP-specific exceptions
                return new ApiResponse { Success = false, Message = httpEx.Message.ToUpper() };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message.ToUpper() };
            }
        }
    }
}
