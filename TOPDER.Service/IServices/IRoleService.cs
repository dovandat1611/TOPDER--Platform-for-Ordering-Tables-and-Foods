using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IRoleService
    {
        Task<bool> AddAsync(RoleDto roleDto);
        Task<bool> UpdateAsync(RoleDto roleDto);
        Task<PaginatedList<RoleDto>> GetPagingAsync(int pageNumber, int pageSize);

    }
}
