using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.IServices;
using TOPDER.Service.Common.ServiceDefinitions;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Dtos.User;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Contact;
using Microsoft.EntityFrameworkCore;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Dtos.Order;

namespace TOPDER.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User> AddAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            return await _userRepository.CreateAndReturnAsync(user);
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest changePassword)
        {
            // Lấy user theo Uid
            var user = await _userRepository.GetByIdAsync(changePassword.Uid);

            if (user == null)
                return false; // Không tìm thấy user

            // Kiểm tra mật khẩu cũ có khớp không
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(changePassword.OldPassword, user.Password);

            if (!isOldPasswordValid)
                return false; // Mật khẩu cũ không đúng

            // Băm mật khẩu mới và cập nhật
            user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);

            // Lưu thay đổi vào cơ sở dữ liệu
            var result = await _userRepository.UpdateAsync(user);

            return result;
        }

        public async Task<bool> CheckExistEmail(string email)
        {
            var query = await _userRepository.QueryableAsync();
            bool exists = await query.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            return exists;
        }

        public async Task<UserOrderIsBalance> GetInformationUserOrderIsBalance(int id)
        {
            var query = await _userRepository.QueryableAsync();

            var user = await query
                .Include(u => u.Wallets) 
                .FirstOrDefaultAsync(u => u.Uid == id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Người dùng với ID {id} không tìm thấy.");
            }

            var walletId = user.Wallets?.Select(x => x.WalletId).FirstOrDefault() ?? 0; 

            var userOrderIsBalance = new UserOrderIsBalance
            {
                Id = user.Uid,
                WalletId = walletId,
            };

            return userOrderIsBalance;
        }


        public async Task<UserPayment> GetInformationUserToPayment(int id)
        {
            var query = await _userRepository.QueryableAsync();

            var user = await query
                .Include(u => u.Customer)  
                .Include(u => u.Restaurant) 
                .FirstOrDefaultAsync(u => u.Uid == id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Người dùng với ID {id} không tìm thấy.");
            }

            string name;
            if (user.Customer != null)
            {
                name = user.Customer.Name; 
            }
            else if (user.Restaurant != null)
            {
                name = user.Restaurant.NameRes; 
            }
            else
            {
                name = Is_Null.ISNULL; 
            }

            var userPayment = new UserPayment
            {
                Id = user.Uid, 
                Name = name 
            };

            return userPayment;
        }

        public async Task<GetRoleAndBalanceForProfileDto> GetRoleUserProfile(int uid)
        {
            var query = await _userRepository.QueryableAsync();

            var user = await query
                .Include(x => x.Role)
                .Include(x => x.Wallets)
                .SingleOrDefaultAsync(u => u.Uid == uid);

            if (user == null)
            {
                return null; // Hoặc throw exception nếu cần
            }

            GetRoleAndBalanceForProfileDto forProfileDto = new GetRoleAndBalanceForProfileDto()
            {
                Role = user.Role?.Name ?? "No Role", // Kiểm tra null cho Role
                WalletBalance = user.Wallets?.FirstOrDefault()?.WalletBalance ?? 0 // Kiểm tra null cho Wallet
            };

            return forProfileDto;
        }


        public async Task<UserLoginDTO> GetUserByEmail(string email)
        {
            var users = await _userRepository.QueryableAsync();

            var user = await users
                .Include(x => x.Role)
                .Include(x => x.Admin)
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .SingleOrDefaultAsync(u => u.Email == email);

            return _mapper.Map<UserLoginDTO>(user);
        }


        public async Task<CheckValidateUserLoginGG> GetUserByEmailToLoginGoogle(int uid)
        {
            var user = await _userRepository.GetByIdAsync(uid);

            return _mapper.Map<CheckValidateUserLoginGG>(user);
        }


        public async Task<UserLoginDTO> GetUserByEmailAndPassword(LoginModel loginModel)
        {
            var users = await _userRepository.QueryableAsync();

            var user = await users
                .Include(x => x.Role)
                .Include(x => x.Admin)
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .SingleOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không hợp lệ.");
            }

            if(user.IsVerify == false)
            {
                throw new UnauthorizedAccessException("Email chưa được Verify.");
            }

            if (user.Status.Equals(Common_Status.INACTIVE))
            {
                throw new UnauthorizedAccessException("Tài khoản hiện không thể đăng nhập vào hệ thống.");
            }

            return _mapper.Map<UserLoginDTO>(user);
        }


        public async Task<List<UserLoginDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.QueryableAsync();

            var user = await users
                .Include(x => x.Role)
                .Include(x => x.Admin)
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .OrderByDescending(x => x.Uid)
                .ToListAsync();

            return _mapper.Map<List<UserLoginDTO>>(user);
        }

        public async Task<bool> Verify(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsVerify)
            {
                return false; 
            }
            user.IsVerify = true;
            return await _userRepository.UpdateAsync(user);
        }

    }
}
