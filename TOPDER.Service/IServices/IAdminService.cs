using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.BlogGroup;

namespace TOPDER.Service.IServices
{
    public interface IAdminService
    {
        Task<bool> UpdateAsync(AdminDto adminDto);
    }
}
