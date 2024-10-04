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

        public async Task<UserOrderIsBalance> GetInformationUserOrderIsBalance(int id)
        {
            var query = await _userRepository.QueryableAsync();

            var user = await query
                .Include(u => u.Customer)
                .Include(u => u.Wallets) 
                .FirstOrDefaultAsync(u => u.Uid == id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Người dùng với ID {id} không tìm thấy.");
            }

            string name = user.Customer?.Name ?? Is_Null.ISNULL; 

            var walletId = user.Wallets?.Select(x => x.WalletId).FirstOrDefault() ?? 0; 

            var userOrderIsBalance = new UserOrderIsBalance
            {
                Id = user.Uid,
                WalletId = walletId,
                Name = name
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
