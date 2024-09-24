using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.ExternalLogin;
using TOPDER.Service.Dtos.Feedback;

namespace TOPDER.Service.IServices
{
    public interface IExternalLoginService
    {
        Task<bool> AddAsync(ExternalLoginDto externalLoginDto);
        Task<bool> UpdateAsync(ExternalLoginDto externalLoginDto);
    }
}
