using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
     public class IdentityService
    {
        private readonly HttpClient _httpClient;
        private JwtService _jwtService;
        private UserService _userService;

        public IdentityService (HttpClient httpClient, JwtService jwtService, UserService userService)
        {
            _httpClient = httpClient;
            _jwtService = jwtService;
            _userService = userService;
        }
        public async Task<ApiResponse> CheckAccessToken(string accessToken)
        {
            try
            {
                // Kiểm tra token trên Google API
                var tokenInfoUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={accessToken}";
                var tokenResponse = await _httpClient.GetAsync(tokenInfoUrl);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = errorContent.ToUpper() };
                }

                // Lấy thông tin người dùng từ Google API
                var userInfoUrl = $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={accessToken}";
                var userInfoResponse = await _httpClient.GetAsync(userInfoUrl);

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    var userInfoError = await userInfoResponse.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = userInfoError.ToUpper() };
                }

                // Phân tích dữ liệu JSON từ phản hồi
                var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
                var user = JObject.Parse(userInfo);

                var userResult = new
                {
                    Id = user["id"]?.ToString(),
                    Email = user["email"]?.ToString(),
                    Name = user["name"]?.ToString(),
                    Picture = user["picture"]?.ToString()
                };

                // Kiểm tra xem người dùng đã tồn tại trong hệ thống hay chưa
                var existingUser = await _userService.GetUserByEmail(userResult.Email);
                var handler = new JwtSecurityTokenHandler();

                if (existingUser != null)
                {
                    // Người dùng đã tồn tại, tạo token và trả về
                    var generatedToken = _jwtService.GenerateToken(existingUser);
                    return new ApiResponse
                    {
                        Success = true,
                        Data = generatedToken,
                        Message = "Authentication success"
                    };
                }

                // Người dùng chưa tồn tại, tạo mới
                var userDto = new UserDto
                {
                    Email = userResult.Email,
                    RoleId = 1,
                    IsVerify = true,
                    Status = "1",
                    CreatedAt = DateTime.UtcNow,
                    IsExternalLogin = true
                };

                await _userService.AddAsync(userDto);

                var newUserLoginDto = new UserLoginDTO
                {
                    CreateDate = userDto.CreatedAt ?? DateTime.UtcNow,
                    Email = userResult.Email,
                    RoleId = userDto.RoleId,
                    Status = userDto.Status,
                };

                // Tạo token mới cho người dùng mới
                var token = _jwtService.GenerateToken(newUserLoginDto);
                return new ApiResponse
                {
                    Success = true,
                    Data = token,
                    Message = "Authentication success"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message.ToUpper() };
            }
        }
    }
}
