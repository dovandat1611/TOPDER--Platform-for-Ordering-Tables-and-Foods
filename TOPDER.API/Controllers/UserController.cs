using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private JwtService _jwtService; // Sử dụng interface cho JwtService

        public UserController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService; // Khởi tạo JwtService
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            var u = await _userService.GetItemAsync(user.Username, user.Password);
            if (u == null) // Kiểm tra người dùng
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid username/password"
                });
            }

            // Giả sử bạn có thông tin roleId từ user
            var token = _jwtService.GenerateToken(u);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authentication success",
                Data = token
            });
        }
    }
}