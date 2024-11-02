using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Excel;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TOPDER.Service.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMapper _mapper;
        private readonly IMenuRepository _menuRepository;
        private readonly IExcelService _excelService;
        private readonly IOrderMenuRepository _orderMenuRepository;
        private readonly ICategoryMenuRepository _categoryMenuRepository;



        public MenuService(IMenuRepository menuRepository, IMapper mapper, 
            IExcelService excelService, IOrderMenuRepository orderMenuRepository, ICategoryMenuRepository categoryMenuRepository)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
            _excelService = excelService;
            _orderMenuRepository = orderMenuRepository;
            _categoryMenuRepository = categoryMenuRepository;
        }
        public async Task<bool> AddAsync(MenuDto menuDto)
        {
            var menu = _mapper.Map<Menu>(menuDto);
            menu.Status = Common_Status.ACTIVE;
            return await _menuRepository.CreateAsync(menu);
        }

        public async Task<bool> AddRangeExcelAsync(CreateExcelMenuDto createExcelMenuDto)
        {
            if (createExcelMenuDto.File == null || createExcelMenuDto.File.Length == 0)
            {
                return false;
            }

            try
            {
                var columnConfigurations = new List<ExcelColumnConfiguration>
                    {
                        new ExcelColumnConfiguration { ColumnName = "DishName", Position = 1, IsRequired = true },
                        new ExcelColumnConfiguration { ColumnName = "Price", Position = 2, IsRequired = true },
                        new ExcelColumnConfiguration { ColumnName = "Description", Position = 3, IsRequired = false },
                        new ExcelColumnConfiguration { ColumnName = "CategoryMenuId", Position = 4, IsRequired = false },

                    };

                var data = await _excelService.ReadFromExcelAsync(createExcelMenuDto.File, columnConfigurations);

                var menuItems = data
                    .Where(row => row != null && row.ContainsKey("DishName") && row.ContainsKey("Price"))
                    .Select(row => new Menu
                    {
                        RestaurantId = createExcelMenuDto.RestaurantId,
                        DishName = row["DishName"],
                        Price = Convert.ToDecimal(row["Price"]),
                        Description = row.TryGetValue("Description", out var description) ? description : null,
                        CategoryMenuId = row.ContainsKey("CategoryMenuId") && !string.IsNullOrEmpty(row["CategoryMenuId"]) ? (int?)Convert.ToInt32(row["CategoryMenuId"]) : null, 
                        Status = Common_Status.ACTIVE
                    })
                    .ToList();

                if (menuItems.Count > 0)
                {
                    await _menuRepository.CreateRangeAsync(menuItems);
                }

                return true;
            }
            catch (Exception)
            {   
                return false;
            }
        }


        public async Task<MenuRestaurantDto> GetItemAsync(int id, int restaurantId)
        {
            var menu = await _menuRepository.GetByIdAsync(id);

            if (menu == null)
            {
                throw new KeyNotFoundException($"Menu với id {id} không tồn tại.");
            }

            if (menu.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Menu với id {id} không thuộc nhà hàng với id {restaurantId}.");
            }

            return _mapper.Map<MenuRestaurantDto>(menu);
        }


        public async Task<bool> InvisibleAsync(int id, int restaurantId)
        {
            var menu = await _menuRepository.GetByIdAsync(id);

            if (menu == null || menu.RestaurantId != restaurantId)
            {
                return false; 
            }

            menu.IsVisible = false;

            return await _menuRepository.UpdateAsync(menu);
        }


        public async Task<List<MenuCustomerByCategoryMenuDto>> ListMenuCustomerByCategoryMenuAsync(int restaurantId)
        {
            // Lấy tất cả các menu của nhà hàng có trạng thái ACTIVE
            var menus = await _menuRepository.QueryableAsync();
            var filteredMenus = await menus
                .Include(m => m.CategoryMenu) // Kèm thông tin CategoryMenu
                .Where(m =>
                    m.RestaurantId == restaurantId &&
                    m.Status == Common_Status.ACTIVE)
                .ToListAsync();

            // Nhóm các menu theo CategoryMenuId
            var groupedMenus = filteredMenus
                .GroupBy(m => m.CategoryMenuId)
                .Select(g => new MenuCustomerByCategoryMenuDto
                {
                    CategoryMenuId = g.Key,
                    CategoryMenuName = g.FirstOrDefault()?.CategoryMenu?.CategoryMenuName,
                    MenusOfCategoryMenu = g.Select(menu => _mapper.Map<MenuCustomerDto>(menu)).ToList()
                })
                .ToList();

            return groupedMenus;
        }


        public async Task<PaginatedList<MenuRestaurantDto>> ListRestaurantPagingAsync(
            int pageNumber,
            int pageSize,
            int restaurantId,
            int? categoryMenuId,
            string? menuName)
        {
            var queryable = await _menuRepository.QueryableAsync();

            var query = queryable
                .Include (o => o.CategoryMenu)
                .Where(x => x.RestaurantId == restaurantId);

            if (categoryMenuId.HasValue && categoryMenuId.Value > 0)
            {
                query = query.Where(x => x.CategoryMenuId == categoryMenuId.Value);
            }

            if (!string.IsNullOrEmpty(menuName))
            {
                query = query.Where(x => x.DishName != null && x.DishName.Contains(menuName));
            }

            var queryDTO = query.Select(r => _mapper.Map<MenuRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<MenuRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(MenuDto menuDto)
        {
            var existingMenu = await _menuRepository.GetByIdAsync(menuDto.MenuId);
            if (existingMenu == null || existingMenu.RestaurantId != menuDto.RestaurantId)
            {
                return false;
            }
            var menu = _mapper.Map<Menu>(menuDto);
            return await _menuRepository.UpdateAsync(menu);
        }
    }
}
