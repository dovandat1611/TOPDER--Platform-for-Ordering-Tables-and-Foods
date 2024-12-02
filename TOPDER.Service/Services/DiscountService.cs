using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.DiscountMenu;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static System.Formats.Asn1.AsnWriter;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IMapper _mapper;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountMenuRepository _discountMenuRepository;
        private readonly IOrderService _orderService;



        public DiscountService(IDiscountRepository discountRepository, IMapper mapper,
            IDiscountMenuRepository discountMenuRepository, IOrderService orderService)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
            _discountMenuRepository = discountMenuRepository;
            _orderService = orderService;
        }
        public async Task<bool> AddAsync(DiscountDto discountDto)
        {
            if (discountDto.StartDate >= discountDto.EndDate)
            {
                throw new ArgumentException("StartDate must be earlier than EndDate.");
            }

            if (discountDto.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE && discountDto.MinOrderValue > discountDto.MaxOrderValue)
            {
                throw new ArgumentException("MinOrderValue cannot be greater than MaxOrderValue.");
            }

            Discount discount = new Discount()
            {   
                DiscountId = 0,
                RestaurantId = discountDto.RestaurantId,
                DiscountName = discountDto.DiscountName,
                IsActive = discountDto.IsActive,
                Quantity = discountDto.Quantity,
                StartDate = discountDto.StartDate,
                EndDate = discountDto.EndDate,
                Description = discountDto.Description,
                ApplicableTo = discountDto.ApplicableTo,
                ApplyType = discountDto.ApplyType,
                Scope = discountDto.Scope
            };

            if(discountDto.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE)
            {
                discount.MinOrderValue = discountDto.MinOrderValue;
                discount.MaxOrderValue = discountDto.MaxOrderValue;
            }

            if (discountDto.Scope == DiscountScope.ENTIRE_ORDER)
            {
                discount.DiscountPercentage = discountDto.DiscountPercentage;
            }

             var createDiscount = await _discountRepository.CreateAndReturnAsync(discount);

            if(createDiscount != null)
            {
                if (discountDto.Scope == DiscountScope.PER_SERVICE)
                {
                    List<DiscountMenu> discountMenus = new List<DiscountMenu>();
                    foreach (var item in discountDto.discountMenuDtos)
                    {
                        DiscountMenu discountMenu = new DiscountMenu()
                        {
                            DiscountMenuId = 0,
                            DiscountId = createDiscount.DiscountId,
                            MenuId = item.MenuId,
                            DiscountMenuPercentage = item.DiscountMenuPercentage,
                        };
                        discountMenus.Add(discountMenu);
                    }
                    return await _discountMenuRepository.CreateRangeAsync(discountMenus);
                }
                if(discountDto.Scope == DiscountScope.ENTIRE_ORDER)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<AvailableDiscountDto>> GetAvailableDiscountsAsync(int restaurantId, int customerId, decimal totalPrice)
        {
            var queryableDiscounts = await _discountRepository.QueryableAsync();
            var queryableDiscountMenu = await _discountMenuRepository.QueryableAsync();

            // Lọc discount theo điều kiện ban đầu
            var query = queryableDiscounts.Where(x => x.RestaurantId == restaurantId
                                                     && x.Quantity > 0
                                                     && x.IsActive == true 
                                                     && x.StartDate <= DateTime.Now
                                                     && x.EndDate >= DateTime.Now
                                                     && x.IsVisible == true);

            // Kiểm tra discount cho khách hàng mới
            if (query.Any(x => x.ApplicableTo == DiscountApplicableTo.NEW_CUSTOMER))
            {
                bool isFirstOrder = await _orderService.CheckIsFirstOrderAsync(customerId, restaurantId);
                if (!isFirstOrder)
                {
                    query = query.Where(x => x.ApplicableTo != DiscountApplicableTo.NEW_CUSTOMER);
                }
            }

            // Kiểm tra discount cho khách hàng trung thành
            if (query.Any(x => x.ApplicableTo == DiscountApplicableTo.LOYAL_CUSTOMER))
            {
                bool isLoyalCustomer = await _orderService.CheckIsLoyalCustomerAsync(customerId, restaurantId);
                if (!isLoyalCustomer)
                {
                    query = query.Where(x => x.ApplicableTo != DiscountApplicableTo.LOYAL_CUSTOMER);
                }
            }

            // Lấy danh sách discount hợp lệ
            var validDiscounts = await query.ToListAsync();

            // Khởi tạo danh sách menuDiscounts
            List<DiscountMenuForAvailableDiscountDto> menuDiscounts = new List<DiscountMenuForAvailableDiscountDto>();

            // Kiểm tra và thêm discount vào danh sách
            var discountDtos = new List<AvailableDiscountDto>();
            foreach (var item in validDiscounts)
            {
                if (item.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE
                    && (item.MaxOrderValue < totalPrice || item.MinOrderValue > totalPrice))
                {
                    continue; // Bỏ qua nếu không thỏa mãn điều kiện ORDER_VALUE_RANGE
                }

                // Nếu discount có Scope là PER_SERVICE, lấy các menu discounts liên quan
                if (item.Scope == DiscountScope.PER_SERVICE)
                {
                    var discountIds = validDiscounts
                        .Select(x => x.DiscountId).ToList();
                    menuDiscounts = await queryableDiscountMenu
                        .Include(x => x.Menu)
                        .Where(x => discountIds.Contains(x.DiscountId))
                        .Select(x => new DiscountMenuForAvailableDiscountDto
                        {
                            DiscountMenuId = x.DiscountMenuId,
                            DiscountId = x.DiscountId,
                            MenuId = x.MenuId,
                            DishName = x.Menu.DishName,
                            Price = x.Menu.Price,
                            Image = x.Menu.Image,
                            DiscountMenuPercentage = x.DiscountMenuPercentage
                        })
                        .ToListAsync();
                }

                // Thêm discount vào danh sách
                discountDtos.Add(new AvailableDiscountDto
                {
                    DiscountId = item.DiscountId,
                    RestaurantId = item.RestaurantId,
                    DiscountPercentage = item.Scope == DiscountScope.ENTIRE_ORDER ? item.DiscountPercentage : null,
                    DiscountName = item.DiscountName,
                    ApplicableTo = item.ApplicableTo,
                    ApplyType = item.ApplyType,
                    MinOrderValue = item.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE ? item.MinOrderValue : null,
                    MaxOrderValue = item.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE ? item.MaxOrderValue : null,
                    Scope = item.Scope,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Description = item.Description,
                    IsActive = item.IsActive,
                    Quantity = item.Quantity,
                    discountMenuDtos = item.Scope == DiscountScope.PER_SERVICE ? menuDiscounts : new List<DiscountMenuForAvailableDiscountDto>()
                });
            }
            return discountDtos;
        }


        public async Task<AvailableDiscountDto> GetItemAsync(int id, int restaurantId)
        {
            var query = await _discountRepository.GetByIdAsync(id);

            if (query == null)
            {
                throw new KeyNotFoundException($"Discount với id {id} không tồn tại.");
            }

            if (query.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException($"Discount với id {id} không thuộc về nhà hàng với id {restaurantId}.");
            }

            var discount = _mapper.Map<AvailableDiscountDto>(query);

            if(discount.Scope == DiscountScope.PER_SERVICE)
            {
                var queryableDiscountMenu = await _discountMenuRepository.QueryableAsync();

                var menuDiscounts = await queryableDiscountMenu
                    .Include(x => x.Menu)
                    .Where(x => x.DiscountId == discount.DiscountId)
                    .Select(x => new DiscountMenuForAvailableDiscountDto
                    {
                        DiscountMenuId = x.DiscountMenuId,
                        DiscountId = x.DiscountId,
                        MenuId = x.MenuId,
                        DishName = x.Menu.DishName,
                        Price = x.Menu.Price,
                        Image = x.Menu.Image,
                        DiscountMenuPercentage = x.DiscountMenuPercentage
                    })
                    .ToListAsync();

                discount.discountMenuDtos = menuDiscounts;
            }

            return discount;
        }


        public async Task<List<AvailableDiscountDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _discountRepository.QueryableAsync();
            var queryableDiscountMenu = await _discountMenuRepository.QueryableAsync();

            var query = queryable
                .Where(x => x.RestaurantId == restaurantId && x.IsVisible == true)
                .OrderByDescending(x => x.DiscountId);

            var queryDTO = _mapper.Map<List<AvailableDiscountDto>>(query);

            // Get discount IDs where the scope is PER_SERVICE
            var discountScope = await query
                .Where(x => x.Scope == DiscountScope.PER_SERVICE)
                .Select(x => x.DiscountId)
                .ToListAsync();

            // Fetch related menu discounts
            var menuDiscounts = await queryableDiscountMenu
                .Include(x => x.Menu)
                .Where(x => discountScope.Contains(x.DiscountId))  // Fixed this line
                .Select(x => new DiscountMenuForAvailableDiscountDto
                {
                    DiscountMenuId = x.DiscountMenuId,
                    DiscountId = x.DiscountId,
                    MenuId = x.MenuId,
                    DishName = x.Menu.DishName,
                    Price = x.Menu.Price,
                    Image = x.Menu.Image,
                    DiscountMenuPercentage = x.DiscountMenuPercentage
                })
                .ToListAsync();

            // Group menuDiscounts by DiscountId
            var discountMenuGroups = menuDiscounts
                .GroupBy(md => md.DiscountId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Assign menu discounts to each discount DTO
            foreach (var discount in queryDTO)
            {
                if (discountMenuGroups.TryGetValue(discount.DiscountId, out var menus))
                {
                    discount.discountMenuDtos = menus;
                }
                else
                {
                    discount.discountMenuDtos = new List<DiscountMenuForAvailableDiscountDto>();
                }
            }

            return queryDTO;
        }

        public async Task<bool> InvisibleAsync(int id, int restaurantId)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null || discount.RestaurantId != restaurantId)
            {
                return false;
            }

            discount.IsVisible = false;
            var result = await _discountRepository.UpdateAsync(discount);
            return result;
        }

        public async Task<bool> ActiveAsync(ActiveDiscountDto activeDiscount)
        {
            var discount = await _discountRepository.GetByIdAsync(activeDiscount.Id);
            if (discount == null || discount.RestaurantId != activeDiscount.RestaurantId)
            {
                return false;
            }
            discount.IsActive = activeDiscount.IsActive;
            var result = await _discountRepository.UpdateAsync(discount);
            return result;
        }


        public async Task<bool> UpdateAsync(DiscountDto discountDto)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(discountDto.DiscountId);
            if (existingDiscount == null || discountDto.RestaurantId != existingDiscount.RestaurantId)
            {
                return false;
            }

            existingDiscount.RestaurantId = discountDto.RestaurantId;
            existingDiscount.ApplicableTo = discountDto.ApplicableTo;
            existingDiscount.ApplyType = discountDto.ApplyType;
            existingDiscount.Scope = discountDto.Scope;
            existingDiscount.StartDate = discountDto.StartDate;
            existingDiscount.EndDate = discountDto.EndDate;
            existingDiscount.Quantity = discountDto.Quantity;
            existingDiscount.DiscountName = discountDto.DiscountName;
            existingDiscount.Description = discountDto.Description;
            existingDiscount.IsActive = discountDto.IsActive;

            if(discountDto.Scope == DiscountScope.PER_SERVICE)
            {
                existingDiscount.DiscountPercentage = null;
                
                // Xóa đi cái cũ
                var query = await _discountMenuRepository.QueryableAsync();
                var getMenuDiscount = await query.Where(x => x.DiscountId == existingDiscount.DiscountId).ToListAsync();

                if(getMenuDiscount.Any())
                {
                    await _discountMenuRepository.DeleteRangeAsync(getMenuDiscount);
                }

                // Làm lại cái mới
                List<DiscountMenu> discountMenus = new List<DiscountMenu>();
                foreach (var item in discountDto.discountMenuDtos)
                {
                    DiscountMenu discountMenu = new DiscountMenu()
                    {
                        DiscountMenuId = 0,
                        DiscountId = existingDiscount.DiscountId,
                        MenuId = item.MenuId,
                        DiscountMenuPercentage = item.DiscountMenuPercentage,
                    };
                    discountMenus.Add(discountMenu);
                }
                 await _discountMenuRepository.CreateRangeAsync(discountMenus);
            }

            if (discountDto.Scope == DiscountScope.ENTIRE_ORDER)
            {

                existingDiscount.DiscountPercentage = discountDto.DiscountPercentage;

                // Xóa đi cái cũ
                var query = await _discountMenuRepository.QueryableAsync();
                var getMenuDiscount = await query.Where(x => x.DiscountId == existingDiscount.DiscountId).ToListAsync();

                if (getMenuDiscount.Any())
                {
                    await _discountMenuRepository.DeleteRangeAsync(getMenuDiscount);
                }

            }

            if (discountDto.ApplyType == DiscountApplyType.ORDER_VALUE_RANGE)
            {
                existingDiscount.MinOrderValue = discountDto.MinOrderValue;
                existingDiscount.MaxOrderValue = discountDto.MaxOrderValue;
            }

            return await _discountRepository.UpdateAsync(existingDiscount);
        }

    }
}
