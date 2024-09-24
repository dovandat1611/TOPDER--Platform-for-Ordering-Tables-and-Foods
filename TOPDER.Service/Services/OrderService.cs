using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public Task<bool> AddAsync(OrderDto orderDto)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<OrderDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> GetItemAsync(int id, int Uid)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<OrderDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(OrderDto orderDto)
        {
            throw new NotImplementedException();
        }
    }
}
