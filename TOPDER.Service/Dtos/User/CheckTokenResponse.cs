using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class CheckTokenResponse
    {
        [JsonPropertyName("User")]
        public UserModel User { get; set; } = new UserModel();

        [JsonPropertyName("RoleName")]
        public string? RoleName { get; set; } = string.Empty;
    }
}
