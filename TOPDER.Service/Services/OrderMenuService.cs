using AutoMapper;
using MailKit.Search;
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
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

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

        public async Task<bool> AddRangeAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            var orderMenuDtoList = _mapper.Map<List<OrderMenu>>(orderMenuDtos);
            return await _orderMenuRepository.CreateRangeAsync(orderMenuDtoList);
        }

        public async Task<bool> ChangeMenusAsync(int orderId, List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            var queryable = await _orderMenuRepository.QueryableAsync();

            var currentOrderMenus = await queryable
                        .Include(x => x.Menu)
                        .Where(x => x.OrderId == orderId).ToListAsync();

            if (currentOrderMenus.Any())
            {
                await _orderMenuRepository.DeleteRangeAsync(currentOrderMenus);
            }

            var orderMenuDtoList = _mapper.Map<List<OrderMenu>>(orderMenuDtos);
            return await _orderMenuRepository.CreateRangeAsync(orderMenuDtoList);
        }

        public async Task<bool> AddMenusAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            var orderMenuDtoList = _mapper.Map<List<OrderMenu>>(orderMenuDtos);
            return await _orderMenuRepository.CreateRangeAsync(orderMenuDtoList);
        }


        public async Task<List<OrderMenuDto>> GetItemsAddByOrderAsync(int id)
        {
            var queryable = await _orderMenuRepository.QueryableAsync();

            var query = await queryable
                .Include(x => x.Menu)
                .Where(x => x.OrderId == id && x.OrderMenuType == OrderMenu_Type.ADD).ToListAsync();

            var queryDTO = _mapper.Map<List<OrderMenuDto>>(query);

            return queryDTO;
        }

        public async Task<List<OrderMenuDto>> GetItemsOriginalByOrderAsync(int id)
        {
            var queryable = await _orderMenuRepository.QueryableAsync();

            var query = await queryable
                .Include(x => x.Menu)
                .Where(x => x.OrderId == id && x.OrderMenuType == OrderMenu_Type.ORIGINAL).ToListAsync();

            var queryDTO = _mapper.Map<List<OrderMenuDto>>(query);

            return queryDTO;
        }

        public async Task<List<OrderMenuDto>> GetItemsByOrderAsync(int id)
        {
            var queryable = await _orderMenuRepository.QueryableAsync();

            var query = await queryable
                .Include(x => x.Menu)
                .Where(x => x.OrderId == id).ToListAsync();

            var queryDTO = _mapper.Map<List<OrderMenuDto>>(query);

            return queryDTO;
        }

    }
}
