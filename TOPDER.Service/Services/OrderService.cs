using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Menu;
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

        public async Task<bool> AddAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.CreateAsync(order);
        }

        public async Task<PaginatedList<OrderCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var queryable = await _orderRepository.QueryableAsync();

            var query = queryable.Where(x => x.CustomerId == customerId);

            var queryDTO = query.Select(r => _mapper.Map<OrderCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<OrderCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<OrderDto> GetItemAsync(int id, int Uid)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Menu với id {id} không tồn tại.");
            }

            if (order.RestaurantId != Uid && order.CustomerId != Uid)
            {
                throw new UnauthorizedAccessException($"Order với id {id} không thuộc user với id {Uid}.");
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<PaginatedList<OrderDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _orderRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<OrderDto>(r));

            var paginatedDTOs = await PaginatedList<OrderDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(OrderDto orderDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(orderDto.OrderId);
            if (existingOrder == null)
            {
                return false;
            }
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.UpdateAsync(order);
        }
    }
}
