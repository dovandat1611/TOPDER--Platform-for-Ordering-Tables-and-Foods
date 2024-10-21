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
                                                     && x.EndDate >= DateTime.Now);

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


        public async Task<DiscountDto> GetItemAsync(int id, int restaurantId)
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
            var discount = _mapper.Map<DiscountDto>(query);
            return discount;
        }

        public async Task<PaginatedList<DiscountDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _discountRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId).OrderByDescending(x => x.DiscountId);

            var queryDTO = query.Select(r => _mapper.Map<DiscountDto>(r));

            var paginatedDTOs = await PaginatedList<DiscountDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id, int restaurantId)
        {
            var feedback = await _discountRepository.GetByIdAsync(id);
            if (feedback == null || feedback.RestaurantId != restaurantId)
            {
                return false;
            }
            var result = await _discountRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> UpdateAsync(DiscountDto discountDto)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(discountDto.DiscountId);
            if (existingDiscount == null || discountDto.RestaurantId != existingDiscount.RestaurantId)
            {
                return false;
            }
            var discount = _mapper.Map<Discount>(discountDto);
            return await _discountRepository.UpdateAsync(discount);
        }
    }
}
