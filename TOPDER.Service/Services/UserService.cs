using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.IServices;
using TOPDER.Service.Common.ServiceDefinitions;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using TOPDER.Service.Dtos.User;


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

        public async Task<UserLoginDTO> GetItemAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAndPasswordAsync(username, password);

            

            if (user == null)
            {
                throw new KeyNotFoundException($"User not found.");
            }

            return _mapper.Map<UserLoginDTO>(user);
        }
    }
}
