﻿using AutoMapper;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantPolicyService _restaurantPolicyService;
        private readonly IPolicySystemService _policySystemService;


        public OrderService(IOrderRepository orderRepository, IMapper mapper,
            IRestaurantPolicyService restaurantPolicyService, IPolicySystemService policySystemService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _restaurantPolicyService = restaurantPolicyService;
            _policySystemService = policySystemService;
        }

        public async Task<Order> AddAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.CreateAndReturnAsync(order);
        }

        public async Task<bool> CheckIsFirstOrderAsync(int customerId, int restaurantId)
        {
            var queryable = await _orderRepository.QueryableAsync();
            var isFirstOrder = !await queryable.AnyAsync(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
            return isFirstOrder;
        }

        public async Task<bool> CheckIsLoyalCustomerAsync(int customerId, int restaurantId)
        {
            var queryable = await _orderRepository.QueryableAsync();

            var customerOrderCounts = queryable
                .Where(order => order.RestaurantId == restaurantId) 
                .GroupBy(order => order.CustomerId) 
                .Select(group => new
                {
                    CustomerId = group.Key,
                    OrderCount = group.Count()
                })
                .OrderByDescending(x => x.OrderCount) 
                .Take(4)
                .ToList();

            return customerOrderCounts.Any(x => x.CustomerId == customerId);
        }

        public async Task<List<OrderDto>> GetAdminPagingAsync()
        {
            var queryable = await _orderRepository.QueryableAsync();

            var query = await queryable
                .Include(x => x.OrderMenus)
                .Include(x => x.OrderTables)
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .OrderByDescending(x => x.OrderId)
                .ToListAsync();

            var orderDto = _mapper.Map<List<OrderDto>>(query);
            return orderDto;
        }

        public async Task<PaginatedList<OrderCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId, string? status)
        {
            var queryable = await _orderRepository.QueryableAsync();

            // Lọc theo CustomerId
            var query = queryable
                .Include(x => x.Restaurant)
                .Include(x => x.Feedbacks)
                .Where(x => x.CustomerId == customerId);

            // Kiểm tra nếu có giá trị status thì lọc theo status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.StatusOrder == status); // Điều kiện lọc theo status
            }

            query = query.OrderByDescending(x => x.OrderId);

            // Chuyển sang DTO
            var queryDTO = query.Select(r => _mapper.Map<OrderCustomerDto>(r));

            // Tạo danh sách phân trang
            var paginatedDTOs = await PaginatedList<OrderCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<EmailForOrder> GetEmailForOrderAsync(int orderId, string role)
        {
            var queryable = await _orderRepository.QueryableAsync();
            var order = await queryable
                .Include(x => x.Customer)
                    .ThenInclude(c => c.UidNavigation)  // Bao gồm UidNavigation của Customer
                .Include(x => x.Restaurant)
                    .ThenInclude(r => r.UidNavigation)  // Bao gồm UidNavigation của Restaurant
                .FirstOrDefaultAsync(x => x.OrderId == orderId);


            if (order == null)
            {
                throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không tồn tại.");
            }

            // Khai báo biến để lưu thông tin email và tên
            string email = string.Empty;
            string name = string.Empty;

            // Kiểm tra vai trò và gán giá trị tương ứng
            if (role.Equals(User_Role.CUSTOMER))
            {
                if (order.Customer?.UidNavigation != null)
                {
                    email = order.Customer.UidNavigation.Email;
                    name = order.Customer.Name;
                }
                else
                {
                    throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không có thông tin khách hàng.");
                }
            }
            else if (role.Equals(User_Role.RESTAURANT))
            {
                if (order.Restaurant?.UidNavigation != null)
                {
                    email = order.Restaurant.UidNavigation.Email;
                    name = order.Restaurant.NameRes;
                }
                else
                {
                    throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không có thông tin nhà hàng.");
                }
            }

            // Tạo đối tượng OrderChangeStatusEmail
            var orderChangeStatusEmail = new EmailForOrder()
            {
                OrderId = orderId.ToString(),
                Email = email,
                Name = name,
            };

            // Kiểm tra Email và Name không null
            if (string.IsNullOrEmpty(orderChangeStatusEmail.Email) || string.IsNullOrEmpty(orderChangeStatusEmail.Name))
            {
                throw new InvalidOperationException($"Không có thông tin email hoặc tên cho đơn hàng với ID {orderId}.");
            }

            return orderChangeStatusEmail;
        }
        public async Task<CancelOrderDto> GetInformationForCancelAsync(int userID, int orderID)
        {
            var query = await _orderRepository.QueryableAsync();

            
            // Tìm kiếm đơn hàng theo ID
            var order = query
                .Include(x => x.Restaurant)
                    .ThenInclude(x => x.UidNavigation)
                        .ThenInclude(x => x.Wallets)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.UidNavigation)
                        .ThenInclude(x => x.Wallets)
                .FirstOrDefault(x => x.OrderId == orderID);

            // Ném ra ngoại lệ nếu không tìm thấy đơn hàng
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderID} not found.");
            }

            // Lấy ví của khách hàng
            var wallet = order.Customer.UidNavigation.Wallets.FirstOrDefault(x => x.Uid == order.CustomerId);

            var walletRestaurant = order.Restaurant.UidNavigation.Wallets.FirstOrDefault(x => x.Uid == order.RestaurantId);


            // Xác định vai trò của người dùng
            var isRestaurantUser = order.RestaurantId == userID;
            var role = (isRestaurantUser == true) ? User_Role.RESTAURANT : User_Role.CUSTOMER;

            decimal cancellationFeePercent = 0;

            if (isRestaurantUser == false)
            {
                var restaurantPolicy = await _restaurantPolicyService.GetActivePolicyAsync(order.RestaurantId ?? 0);
                if (restaurantPolicy != null)
                {
                    cancellationFeePercent = restaurantPolicy.CancellationFeePercent ?? 0;
                }
            }

            //if (order.PaidType == Paid_Type.ENTIRE_ORDER)
            //{
            //    totalAmount = order.TotalAmount ?? 0;
            //}
            //if (order.PaidType == Paid_Type.DEPOSIT)
            //{
            //    totalAmount = order.DepositAmount ?? 0;
            //}

            // Tạo đối tượng CancelOrderDto và trả về
            return new CancelOrderDto
            {
                OrderId = orderID,
                CustomerID = order.CustomerId ?? 0,
                RestaurantID = order.RestaurantId ?? 0,

                // Info Restaurant
                EmailRestaurant = order.Restaurant.UidNavigation.Email,
                EmailCustomer = order.Customer.UidNavigation.Email,

                // Info Customer
                NameCustomer = order.Customer.Name,
                NameRestaurant = order.Restaurant.NameRes,

                // User Cancel
                UserCancelID = (isRestaurantUser == true) ? order.RestaurantId ?? 0 : order.CustomerId ?? 0,

                //WalletRestaurant
                WalletRestaurantId = walletRestaurant?.WalletId ?? 0,
                WalletBalanceRestaurant = walletRestaurant?.WalletBalance ?? 0,

                // WalletCustomer
                WalletCustomerId = wallet?.WalletId ?? 0,
                WalletBalanceCustomer = wallet?.WalletBalance ?? 0, // Sử dụng toán tử null-coalescing để xử lý null

              
                // CancellationFeePercent Restaurant
                CancellationFeePercent = (isRestaurantUser == true) ? 100 : cancellationFeePercent,

                TotalAmount = order.TotalAmount,

                TotalPaymentAmount = order.TotalPaymentAmount ?? 0,

                RoleName = role
            };
        }

        public async Task<CompleteOrderDto> GetInformationForCompleteAsync(int orderID)
        {
            var query = await _orderRepository.QueryableAsync();

            var policySystem = await _policySystemService.GetAllAsync();

            policySystem = policySystem.OrderBy(x => x.MinOrderValue).ToList();

            // Tìm kiếm đơn hàng theo ID
            var order = await query
                .Include(x => x.Restaurant)
                    .ThenInclude(x => x.UidNavigation)
                        .ThenInclude(x => x.Wallets)
                .FirstOrDefaultAsync(x => x.OrderId == orderID);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderID} not found.");
            }

            decimal totalAmount = order.TotalPaymentAmount ?? 0;

            //if(order.PaidType == Paid_Type.ENTIRE_ORDER)
            //{
            //    totalAmount = order.TotalAmount ?? 0;
            //}
            //if (order.PaidType == Paid_Type.DEPOSIT)
            //{
            //    totalAmount = order.DepositAmount ?? 0;
            //}

            var wallet = order.Restaurant.UidNavigation.Wallets.FirstOrDefault(x => x.Uid == order.RestaurantId);

            // Kiểm tra phí dịch vụ dựa trên số tiền đơn hàng
            decimal serviceFee = 0;

            if(policySystem != null && policySystem.Any())
            {
                foreach (var policy in policySystem)
                {
                    // Ví dụ: kiểm tra nếu giá trị đơn hàng nằm trong khoảng chính sách
                    if (totalAmount >= policy.MinOrderValue &&
                        (policy.MaxOrderValue == 0 || totalAmount <= policy.MaxOrderValue))
                    {
                        serviceFee = policy.FeeAmount;
                        break; // Áp dụng chính sách đầu tiên phù hợp
                    }
                }
            }

            // Tính số dư ví sau khi trừ phí
            var updatedWalletBalance = wallet.WalletBalance + totalAmount - serviceFee;

            return new CompleteOrderDto
            {
                OrderId = orderID,
                RestaurantID = order.RestaurantId ?? 0,
                WalletId = wallet.WalletId,
                RestaurantName = order.Restaurant.NameRes,
                WalletBalance = updatedWalletBalance ?? 0,
                TotalPaymentAmount = totalAmount - serviceFee,
            };

        }


        public async Task<OrderDto> GetItemAsync(int id, int Uid)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order với id {id} không tồn tại.");
            }

            if (order.RestaurantId != Uid && order.CustomerId != Uid)
            {
                throw new UnauthorizedAccessException($"Order với id {id} không thuộc user với id {Uid}.");
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderPaidEmail> GetOrderPaid(int orderID)
        {
            // Bước 1: Lấy truy vấn ban đầu từ repository
            var queryOrder = await _orderRepository.QueryableAsync();

            // Bước 2: Thêm các Include vào truy vấn
            queryOrder = queryOrder
                .Include(x => x.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t.Room)
                .Include(x => x.Restaurant)
                .Include(x => x.Customer)
                    .ThenInclude(c => c.UidNavigation);

            // Bước 3: Thực hiện truy vấn FirstOrDefault
            var order = await queryOrder.FirstOrDefaultAsync(x => x.OrderId == orderID);

            // Kiểm tra nếu order không tồn tại
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            // Khởi tạo email
            OrderPaidEmail orderPaidEmail = new OrderPaidEmail()
            {
                RestaurantId = order.RestaurantId ?? 0,
                CustomerId = order.CustomerId ?? 0,
                Name = order.Customer.Name,
                Email = order.Customer.UidNavigation.Email,
                RestaurantName = order.Restaurant.NameRes,
                OrderId = order.OrderId.ToString(),
                NumberOfGuests = order.NumberChild + order.NumberPerson,
                ReservationDate = order.DateReservation,
                ReservationTime = order.TimeReservation,
                TotalAmount = order.TotalAmount ?? 0,
                Rooms = new List<RoomEmail>(),
                TableName = new List<string>()
            };

            // Xử lý các bàn và phòng
            foreach (var orderTable in order.OrderTables)
            {
                var table = orderTable.Table;
                var room = table?.Room;

                if (room != null)
                {
                    var existingRoom = orderPaidEmail.Rooms.FirstOrDefault(r => r.RoomName == room.RoomName);
                    if (existingRoom != null)
                    {
                        existingRoom.Tables.Add(table.TableName);
                    }
                    else
                    {
                        var roomEmail = new RoomEmail
                        {
                            RoomName = room.RoomName,
                            Tables = new List<string> { table.TableName }
                        };
                        orderPaidEmail.Rooms.Add(roomEmail);
                    }
                }
                else
                {
                    orderPaidEmail.TableName.Add(table.TableName);
                }
            }

            return orderPaidEmail;
        }


        public async Task<PaginatedList<OrderRestaurantDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, string? status, DateTime? month, DateTime? date)
        {
            var queryable = await _orderRepository.QueryableAsync();

            // Lọc theo restaurantId
            var query = queryable.Include(x => x.OrderTables).Include(x => x.Customer).Include(x => x.Reports).Where(x => x.RestaurantId == restaurantId);

            // Lọc theo status nếu có
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.StatusOrder == status);
            }

            // Lọc theo month nếu có
            if (month != null)
            {
                int selectedMonth = month.Value.Month;
                int selectedYear = month.Value.Year;
                query = query.Where(x => x.CreatedAt.Value.Month == selectedMonth && x.CreatedAt.Value.Year == selectedYear);
            }

            // Lọc theo date nếu có
            if (date != null)
            {
                int selectedDay = date.Value.Day;
                int selectedMonth = date.Value.Month;
                int selectedYear = date.Value.Year;
                query = query.Where(x => x.CreatedAt.Value.Day == selectedDay && x.CreatedAt.Value.Month == selectedMonth && x.CreatedAt.Value.Year == selectedYear);
            }

            query = query.OrderByDescending(x => x.OrderId);

            // Chuyển sang DTO
            var queryDTO = query.Select(r => _mapper.Map<OrderRestaurantDto>(r));

            // Tạo danh sách phân trang
            var paginatedDTOs = await PaginatedList<OrderRestaurantDto>.CreateAsync(
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

        public async Task<bool> UpdatePaidOrderAsync(OrderDto orderDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(orderDto.OrderId);
            if (existingOrder == null)
            {
                return false;
            }

            existingOrder.StatusPayment = orderDto.StatusPayment;
            existingOrder.ContentPayment = orderDto.ContentPayment;
            existingOrder.PaidType = orderDto.PaidType;
            existingOrder.TotalPaymentAmount = orderDto.TotalPaymentAmount;

            return await _orderRepository.UpdateAsync(existingOrder);
        }

        public async Task<bool> UpdatePendingOrdersAsync(MultiStatusOrders multiStatus)
        {
            if (string.IsNullOrEmpty(multiStatus.Status))
            {
                return false;
            }
            int count = 0;
            foreach (var item in multiStatus.OrderID)
            {
                var order = await _orderRepository.GetByIdAsync(item);
                if (order == null)
                {
                    continue;
                }
                if (multiStatus.Status.Equals(Order_Status.CONFIRM))
                {
                    order.StatusOrder = multiStatus.Status;
                    order.ConfirmedAt = DateTime.Now;
                    var checkUpdate = await _orderRepository.UpdateAsync(order);
                    if(checkUpdate == true)
                    {
                        count++;
                    }
                }
            }
            if(count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<OrderDto> UpdateStatusAsync(int orderID, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(order.StatusOrder))
            {
                if (order.StatusOrder.Equals(status))
                {
                    return null;
                }
                if (status.Equals(Order_Status.PAID))
                {
                    order.PaidAt = DateTime.Now;
                }
                if (status.Equals(Order_Status.CONFIRM)) {
                    order.ConfirmedAt = DateTime.Now;
                }
                if (status.Equals(Order_Status.COMPLETE))
                {
                    order.CompletedAt = DateTime.Now;
                }
                if (status.Equals(Order_Status.CANCEL))
                {
                    order.CancelledAt = DateTime.Now;
                }
                order.StatusOrder = status;
                var updateOrder = await _orderRepository.UpdateAsync(order);
                if(updateOrder == true)
                {
                    return _mapper.Map<OrderDto>(order); ;
                }
            }
            return null;
        }

        public async Task<bool> UpdateStatusCancelAsync(int orderID, string status, string cancelReason)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(order.StatusOrder))
            {
                if (order.StatusOrder.Equals(status))
                {
                    return false;
                }
                if (status.Equals(Order_Status.CANCEL))
                {
                    order.CancelledAt = DateTime.Now;
                }
                order.StatusOrder = status;
                order.CancelReason = cancelReason;
                return await _orderRepository.UpdateAsync(order);
            }
            return false;
        }

        public async Task<bool> UpdateStatusOrderPayment(int orderID, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null)
            {
                return false; 
            }

            if (!string.IsNullOrEmpty(order.StatusPayment))
            {
                if (order.StatusPayment.Equals(status))
                {
                    return false; 
                }

                if (status.Equals(Payment_Status.SUCCESSFUL))
                {
                    order.StatusPayment = status;
                    order.StatusOrder = Order_Status.PAID;
                    order.PaidAt = DateTime.Now;
                }
                else
                {
                    order.StatusPayment = status; 
                }
                return await _orderRepository.UpdateAsync(order);
            }
            return false; 
        }

        //public async Task<bool> UpdateTotalIncomeChangeMenuAsync(int orderID, decimal totalAmount)
        //{
        //    var order = await _orderRepository.GetByIdAsync(orderID);

        //    if (order == null) { return false;}

        //    order.TotalAmount = totalAmount;
        //    order.DiscountId = null;

        //    return await _orderRepository.UpdateAsync(order);
        //}

        public async Task<bool> UpdateFoodAmountChangeMenuAsync(int orderID, decimal foodAmount)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null) { return false; }

            order.FoodAmount = foodAmount;

            order.TotalAmount = (foodAmount + (order.DepositAmount ?? 0));

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> UpdateAddFoodAmountChangeMenuAsync(int orderID, decimal addFoodAmount)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null) { return false; }

            order.FoodAddAmount = addFoodAmount;
            order.TotalAmount = (addFoodAmount + (order.DepositAmount ?? 0) + (order.FoodAmount ?? 0));

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> UpdateTotalPaymentAmountAsync(int orderID, decimal totalPaymentAmount)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null) { return false; }

            order.TotalPaymentAmount = totalPaymentAmount;

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderDto> GetOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order với id {id} không tồn tại.");
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
