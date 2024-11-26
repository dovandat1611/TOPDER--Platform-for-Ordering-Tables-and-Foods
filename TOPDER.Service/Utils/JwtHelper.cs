using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.User;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Utils
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(UserLoginDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            // Khởi tạo danh sách claim
            var claims = new List<Claim>
            {
                new Claim("uid", user.Uid.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                new Claim("phone", user.Phone ?? string.Empty),
            };

            if(user.Role == User_Role.CUSTOMER || user.Role == User_Role.ADMIN)
            {
                claims.AddRange(new[]
                {
                new Claim("name", user.Name ?? string.Empty),
                new Claim("dob", user.Dob?.ToString() ?? string.Empty),
                new Claim("image", user.Image ?? string.Empty),
                });
            }

            if(user.Role == User_Role.CUSTOMER)
            {
                claims.Add(new Claim("gender", user.Gender ?? string.Empty));
            }

            if (user.Role == User_Role.RESTAURANT)
            {
                claims.AddRange(new[]
                {
                new Claim("nameOwner", user.NameOwner ?? string.Empty),
                new Claim("nameRes", user.NameRes ?? string.Empty),
                new Claim("categoryRestaurantId", user.CategoryRestaurantId?.ToString() ?? string.Empty),
                new Claim("categoryRestaurantName", user.CategoryRestaurantName ?? string.Empty),
                new Claim("logo", user.Logo ?? string.Empty),
                new Claim("address", user.Address ?? string.Empty),
                new Claim("openTime", user.OpenTime?.ToString() ?? string.Empty),
                new Claim("closeTime", user.CloseTime?.ToString() ?? string.Empty),
                new Claim("subdescription", user.Subdescription ?? string.Empty),
                new Claim("description", user.Description ?? string.Empty),
                new Claim("provinceCity", user.ProvinceCity ?? string.Empty),
                new Claim("district", user.District ?? string.Empty),
                new Claim("commune", user.Commune ?? string.Empty),
                new Claim("discount", user.Discount?.ToString() ?? string.Empty),
                new Claim("price", user.Price?.ToString() ?? string.Empty),
                new Claim("maxCapacity", user.MaxCapacity?.ToString() ?? string.Empty),
                new Claim("isBookingEnabled", user.IsBookingEnabled?.ToString() ?? string.Empty),
                new Claim("firstFeePercent", user.FirstFeePercent?.ToString() ?? string.Empty),
                new Claim("returningFeePercent", user.ReturningFeePercent?.ToString() ?? string.Empty),
                new Claim("cancellationFeePercent", user.CancellationFeePercent?.ToString() ?? string.Empty)
                });
            }

            // Tạo mô tả token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpireHours"])),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Tạo token và trả về chuỗi token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
