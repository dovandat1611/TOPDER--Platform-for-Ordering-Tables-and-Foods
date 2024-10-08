using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.User;

namespace TOPDER.Service.IServices
{
    public interface IIdentityService
    {
        Task<ApiResponse> AuthenticateWithGoogle(string accessToken);
    }
}
