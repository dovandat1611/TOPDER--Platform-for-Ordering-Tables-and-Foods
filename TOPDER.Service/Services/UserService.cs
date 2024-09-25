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
    }
}
