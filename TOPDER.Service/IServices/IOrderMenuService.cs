﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderMenuService
    {
        Task<bool> AddAsync(List<CreateOrUpdateOrderMenuDto> orderMenuDtos);
        Task<List<OrderMenuDto>> GetItemsByOrderAsync(int id);
    }
}
