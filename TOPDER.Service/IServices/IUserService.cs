using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IUserService
    {
        Task<bool> ChangeStatus(int id, string status);
    }   
}
