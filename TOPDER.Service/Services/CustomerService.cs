using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<Customer> AddAsync(CreateCustomerRequest customerRequest)
        {
            var customer = _mapper.Map<Customer>(customerRequest);
            return await _customerRepository.CreateAndReturnAsync(customer);
        }

        public async Task<bool> CheckProfile(int uid)
        {
            var customer = await _customerRepository.GetByIdAsync(uid);

            if (customer is null) return false;

            return !string.IsNullOrWhiteSpace(customer.Name) &&
                   !string.IsNullOrWhiteSpace(customer.Phone) &&
                   customer.Dob.HasValue &&
                   !string.IsNullOrWhiteSpace(customer.Gender);
        }


        public async Task<PaginatedList<CustomerInfoDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _customerRepository.QueryableAsync();

            var queryDTO = query.Select(r => _mapper.Map<CustomerInfoDto>(r));

            var paginatedDTOs = await PaginatedList<CustomerInfoDto>.CreateAsync(
                queryDTO.AsNoTracking(), 
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<CustomerProfileDto?> Profile(int uid)
        {
            var customer = await _customerRepository.GetByIdAsync(uid);

            if (customer == null) return null;

            return _mapper.Map<CustomerProfileDto>(customer);
        }


        public async Task<bool> UpdateProfile(CustomerProfileDto customerProfile)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(customerProfile.Uid);
            if (existingCustomer == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(customerProfile.Name))
            {
                existingCustomer.Name = customerProfile.Name;
            }
            if (customerProfile.Gender != null)
            {
                existingCustomer.Gender = customerProfile.Gender;
            }
            if (!string.IsNullOrEmpty(customerProfile.Phone))
            {
                existingCustomer.Phone = customerProfile.Phone;
            }
            if (customerProfile.Dob != null)
            {
                existingCustomer.Dob = customerProfile.Dob;
            }
            if (!string.IsNullOrEmpty(customerProfile.Image))
            {
                existingCustomer.Image = customerProfile.Image;
            }
            return await _customerRepository.UpdateAsync(existingCustomer);
        }

    }
}
