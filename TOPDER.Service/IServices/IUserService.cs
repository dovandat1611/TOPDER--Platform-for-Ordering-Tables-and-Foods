using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IUserService
    {
        Task<UserLoginDTO> GetUserByEmailAndPassword(LoginModel loginModel);
        Task<GetRoleAndBalanceForProfileDto> GetRoleUserProfile(int uid);
        Task<User> AddAsync(UserDto userDto);
        Task<bool> Verify(int id);
        Task<UserLoginDTO> GetUserByEmail(string email);
        Task<CheckValidateUserLoginGG> GetUserByEmailToLoginGoogle(int uid);
        Task<UserPayment> GetInformationUserToPayment(int id);
        Task<UserOrderIsBalance> GetInformationUserOrderIsBalance(int id);
        Task<bool> CheckExistEmail(string email);
        Task<bool> ChangePassword(ChangePasswordRequest changePassword);
        Task<List<UserLoginDTO>> GetAllUsersAsync();
    }
}
