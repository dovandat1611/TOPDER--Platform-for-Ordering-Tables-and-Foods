using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.Dtos.Log;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ILogService
    {
        Task<bool> AddAsync(LogDto logDto);
        Task<PaginatedList<LogDto>> GetPagingAsync(int pageNumber, int pageSize, int userId);

    }
}
