using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.PolicySystem;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IPolicySystemService
    {
        Task<bool> AddAsync(CreatePolicySystemDto policySystemDto);
        Task<bool> UpdateAsync(UpdatePolicySystemDto policySystemDto);
        Task<bool> RemoveAsync(int id);
        Task<List<PolicySystemDto>> GetAllAsync();
    }
}
