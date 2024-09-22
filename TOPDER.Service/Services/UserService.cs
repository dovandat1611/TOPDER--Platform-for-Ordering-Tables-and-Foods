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

        public async Task<UserLoginDTO> GetUserByEmailAndPassword(string email, string password)
        {
            var users = await _userRepository.QueryableAsync();

            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserLoginDTO>(user);
        }
    }
}
