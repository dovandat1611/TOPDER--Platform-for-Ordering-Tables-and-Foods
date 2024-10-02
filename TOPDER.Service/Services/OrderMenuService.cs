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
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class OrderMenuService : IOrderMenuService
    {
        private readonly IMapper _mapper;
        private readonly IOrderMenuRepository _orderMenuRepository;

        public OrderMenuService(IOrderMenuRepository orderMenuRepository, IMapper mapper)
        {
            _orderMenuRepository = orderMenuRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            var orderMenuDtoList = _mapper.Map<List<OrderMenu>>(orderMenuDtos);
            return await _orderMenuRepository.CreateRangeAsync(orderMenuDtoList);
        }


        public async Task<List<OrderMenuDto>> GetItemsByOrderAsync(int id)
        {
            var queryable = await _orderMenuRepository.QueryableAsync();

            var query = queryable
                .Include(x => x.Menu)
                .Where(x => x.OrderId == id);

            var queryDTO = await query.Select(r => _mapper.Map<OrderMenuDto>(r)).ToListAsync();

            return queryDTO;
        }

    }
}
