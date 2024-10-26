using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Customer;

namespace TOPDER.Service.IServices
{
    public interface IAdminService
    {
        Task<Admin> AddAsync(AdminDto adminDto);
        Task<bool> UpdateAsync(AdminDto adminDto);
        Task<AdminDto?> Profile(int uid);
    }
}
