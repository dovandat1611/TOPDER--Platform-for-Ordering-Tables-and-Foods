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
        private IdentityService _identityService;
        private JwtService _jwtService; // Sử dụng interface cho JwtService

        public UserController(IUserService userService, JwtService jwtService, IdentityService identityService)
        {
            _identityService = identityService;
            _userService = userService;
            _jwtService = jwtService; // Khởi tạo JwtService
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            var u = await _userService.GetUserByEmailAndPassword(user.Email, user.Password);
            if (u == null) 
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "User not found"
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
        [HttpPost("signin-google")]
        public async Task<IActionResult> CheckAccessToken([FromBody] string accessToken)
        {
            var result = await _identityService.CheckAccessToken(accessToken);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}