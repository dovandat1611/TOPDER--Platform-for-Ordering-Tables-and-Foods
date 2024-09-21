using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IUserService
    {
         Task<UserLoginDTO> GetItemAsync(string username, string password);
    }   
}
