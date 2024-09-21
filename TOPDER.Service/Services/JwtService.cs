using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.User;

namespace TOPDER.Service.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserLoginDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("email", user.Email),
                new Claim("roleId", user.RoleId.ToString()),
                new Claim("status", user.Status.ToString()),
                new Claim("createDate", user.CreateDate.ToString("o")), // Định dạng ISO 8601
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID duy nhất cho token
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public object GenerateToken(Task<UserLoginDTO> u)
        {
            throw new NotImplementedException();
        }
    }
}
