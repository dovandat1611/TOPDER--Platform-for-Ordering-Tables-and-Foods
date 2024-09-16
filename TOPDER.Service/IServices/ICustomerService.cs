using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Utils;
using TOPDER.Service.Dtos.Customer;


namespace TOPDER.Service.IServices
{
    public interface ICustomerService
    {
        Task<PaginatedList<CustomerInfoDto>> GetPagingAsync(int pageNumber, int pageSize);
    }
}
