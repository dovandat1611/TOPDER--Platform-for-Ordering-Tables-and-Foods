using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Net.payOS.Types;
using Org.BouncyCastle.Utilities.Encoders;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderMenuService _orderMenuService;
        private readonly IOrderTableService _orderTableService;
        private readonly IWalletService _walletService;
        private readonly IDiscountRepository _discountRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ISendMailService _sendMailService;
        private readonly IDiscountMenuRepository _discountMenuRepository;
        private readonly IConfiguration _configuration;



        public OrderController(IOrderService orderService, IOrderMenuService orderMenuService,
            IWalletService walletService, IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository, IDiscountRepository discountRepository,
            IUserService userService, IWalletTransactionService walletTransactionService,
            IPaymentGatewayService paymentGatewayService, ISendMailService sendMailService,
            IOrderTableService orderTableService, IDiscountMenuRepository discountMenuRepository,
            IConfiguration configuration, IRestaurantService restaurantService)
        {
            _orderService = orderService;
            _orderMenuService = orderMenuService;
            _walletService = walletService;
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
            _discountRepository = discountRepository;
            _userService = userService;
            _walletTransactionService = walletTransactionService;
            _paymentGatewayService = paymentGatewayService;
            _sendMailService = sendMailService;
            _orderTableService = orderTableService;
            _discountMenuRepository = discountMenuRepository;
            _configuration = configuration;
            _restaurantService = restaurantService;
        }


        // ORDER
        // Phương thức tính toán tổng tiền đơn hàng
        [HttpPost("CalculateTotalAmountFreDiscount")]
        [SwaggerOperation(Summary = "Tính toán và trả về TotalAmount: Customer")]
        public async Task<IActionResult> CalculateTotalAmountFreDiscountAsync([FromBody] CaculatorOrderDto caculatorOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(caculatorOrder.RestaurantId);
            if (restaurant == null)
            {
                return NotFound("Nhà hàng không tồn tại.");
            }

            decimal totalAmount = 0;

            // Tính tổng giá nhà hàng và áp dụng giảm giá nếu có
            if (restaurant.Price > 0)
            {
                totalAmount += restaurant.Price;

                if (restaurant.Discount.HasValue && restaurant.Discount > 0)
                {
                    totalAmount *= (1 - (restaurant.Discount.Value / 100m)); // Chia cho 100m để tránh lỗi số học
                }
            }

            // Tính tổng tiền từ thực đơn
            totalAmount += await CalculateMenuTotalForPreOrderAsync(caculatorOrder);

            // Áp dụng phí dựa trên trạng thái khách hàng
            totalAmount = await ApplyCustomerFeeAsync(caculatorOrder.CustomerId, restaurant, totalAmount);

            return Ok(totalAmount); // Trả về tổng số tiền đã tính toán
        }

        // Tạo đơn hàng
        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo đơn hàng | đơn hàng có thể có Table, Menu | sau khi click vào tạo đơn hàng thì sẽ hiện ra Discount: Customer")]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel orderModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(orderModel.RestaurantId);

            if (restaurant == null)
            {
                return NotFound("Nhà hàng không tồn tại.");
            }

            // Nếu đặt bàn miễn phí
            if (restaurant.Price == 0 &&
                (orderModel.OrderMenus == null || !orderModel.OrderMenus.Any()))
            {
                var orderToFree = await CreateFreeOrderAsync(orderModel);
                if (orderToFree != null)
                {
                    if (orderModel.TableIds != null && orderModel.TableIds.Any())
                    {
                        CreateRestaurantOrderTablesDto orderTablesDto = new CreateRestaurantOrderTablesDto()
                        {
                            OrderId = orderToFree.OrderId,
                            TableIds = orderModel.TableIds,
                        };
                        await _orderTableService.AddRangeAsync(orderTablesDto);
                    }
                    await SendOrderEmailAsync(orderToFree.OrderId);
                    return Ok("Tạo đơn hàng miễn phí thành công");
                }
                return BadRequest("Tạo đơn hàng miễn phí thất bại");
            }

            // Tính toán tổng tiền cho đơn hàng
            decimal totalAmount = await CalculateTotalAmountAsync(orderModel, restaurant);

            // Tạo đơn hàng
            var orderDto = new OrderDto
            {
                OrderId = 0,
                CustomerId = orderModel.CustomerId,
                RestaurantId = orderModel.RestaurantId,
                DiscountId = orderModel.DiscountId ?? null,
                CategoryRoomId = orderModel.CategoryRoomId ?? null,
                NameReceiver = orderModel.NameReceiver,
                PhoneReceiver = orderModel.PhoneReceiver,
                TimeReservation = orderModel.TimeReservation,
                DateReservation = orderModel.DateReservation,
                NumberPerson = orderModel.NumberPerson,
                NumberChild = orderModel.NumberChild,
                ContentReservation = orderModel.ContentReservation,
                TypeOrder = Order_Type.RESERVATION,
                TotalAmount = totalAmount,
                StatusOrder = Order_Status.PENDING,
                CreatedAt = DateTime.Now
            };

            var order = await _orderService.AddAsync(orderDto);
            if (order != null)
            {   
                if(orderModel.TableIds != null && orderModel.TableIds.Any())
                {
                    CreateRestaurantOrderTablesDto orderTablesDto = new CreateRestaurantOrderTablesDto()
                    {
                        OrderId = order.OrderId,
                        TableIds = orderModel.TableIds,
                    };
                    await _orderTableService.AddRangeAsync(orderTablesDto);
                }
                if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
                {
                    await AddOrderMenusAsync(order.OrderId, orderModel.OrderMenus);
                }
                await SendOrderEmailAsync(order.OrderId);
                return Ok("Tạo đơn hàng thành công");
            }

            return BadRequest("Tạo đơn hàng thất bại");
        }

        // Phương thức tạo đơn hàng miễn phí
        private async Task<Order> CreateFreeOrderAsync(OrderModel orderModel)
        {
            OrderDto orderDtoToFree = new OrderDto
            {
                OrderId = 0,
                CustomerId = orderModel.CustomerId,
                RestaurantId = orderModel.RestaurantId,
                DiscountId = orderModel.DiscountId ?? null,
                CategoryRoomId = orderModel.CategoryRoomId ?? null,
                NameReceiver = orderModel.NameReceiver,
                PhoneReceiver = orderModel.PhoneReceiver,
                TimeReservation = orderModel.TimeReservation,
                DateReservation = orderModel.DateReservation,
                NumberPerson = orderModel.NumberPerson,
                NumberChild = orderModel.NumberChild,
                ContentReservation = orderModel.ContentReservation,
                TypeOrder = Order_Type.RESERVATION,
                TotalAmount = 0,
                StatusOrder = Order_Status.PENDING,
                CreatedAt = DateTime.Now
            };

            return await _orderService.AddAsync(orderDtoToFree);
        }

        // Phương thức tính toán tổng tiền đơn hàng
        private async Task<decimal> CalculateTotalAmountAsync(OrderModel orderModel, Restaurant restaurant)
        {
            decimal totalAmount = 0;

            // Kiểm tra và áp dụng giảm giá cho nhà hàng
            if (restaurant.Price > 0)
            {
                totalAmount += restaurant.Price;
                if (restaurant.Discount > 0)
                {
                    totalAmount *= (1 - (restaurant.Discount.Value / 100));
                }
            }

            // Kiểm tra và áp dụng giảm giá cho đơn hàng
            if (orderModel.DiscountId.HasValue && orderModel.DiscountId.Value != 0)
            {
                var discount = await _discountRepository.GetByIdAsync(orderModel.DiscountId.Value);
                if (discount != null && discount.IsActive.HasValue && discount.IsActive.Value && discount.Quantity > 0)
                {
                    if (discount.Scope == DiscountScope.ENTIRE_ORDER)
                    {
                        totalAmount += await CalculateMenuTotalAsync(orderModel);
                        totalAmount *= (1 - (discount.DiscountPercentage / 100)) ?? 0;
                        discount.Quantity -= 1;
                        await _discountRepository.UpdateAsync(discount);
                    }
                    else if (discount.Scope == DiscountScope.PER_SERVICE)
                    {
                        totalAmount = await ApplyServiceDiscountAsync(orderModel, discount, totalAmount);
                    }
                }
            }
            else
            {
                totalAmount += await CalculateMenuTotalAsync(orderModel);
            }

            // Áp dụng phí dựa trên trạng thái khách hàng
            totalAmount = await ApplyCustomerFeeAsync(orderModel.CustomerId, restaurant, totalAmount);

            return totalAmount;
        }

        // Phương thức áp dụng giảm giá theo menu
        private async Task<decimal> ApplyServiceDiscountAsync(OrderModel orderModel, Discount discount, decimal totalAmount)
        {
            var discountMenus = await _discountMenuRepository.QueryableAsync();
            var applicableMenus = await discountMenus
                .Where(x => x.DiscountId == discount.DiscountId)
                .ToListAsync();

            if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
            {
                foreach (var orderMenu in orderModel.OrderMenus)
                {
                    var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                    if (menu != null)
                    {
                        var menuDiscount = applicableMenus.FirstOrDefault(x => x.MenuId == menu.MenuId);
                        if (menuDiscount != null)
                        {
                            totalAmount += (menu.Price * (1 - (menuDiscount.DiscountMenuPercentage / 100)) * (orderMenu.Quantity ?? 1));
                        }
                        else
                        {
                            totalAmount += (menu.Price * (orderMenu.Quantity ?? 1));
                        }
                    }
                }
            }
            return totalAmount;
        }

        // Phương thức tính tổng tiền theo menu
        private async Task<decimal> CalculateMenuTotalAsync(OrderModel orderModel)
        {
            decimal totalAmount = 0;
            if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
            {
                foreach (var orderMenu in orderModel.OrderMenus)
                {
                    var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                    if (menu != null)
                    {
                        totalAmount += (menu.Price * (orderMenu.Quantity ?? 1));
                    }
                }
            }
            return totalAmount;
        }

        private async Task<decimal> CalculateMenuTotalForPreOrderAsync(CaculatorOrderDto orderModel)
        {
            decimal totalAmount = 0;
            if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
            {
                foreach (var orderMenu in orderModel.OrderMenus)
                {
                    var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                    if (menu != null)
                    {
                        totalAmount += (menu.Price * (orderMenu.Quantity ?? 1));
                    }
                }
            }
            return totalAmount;
        }

        // Phương thức áp dụng phí dựa trên trạng thái khách hàng
        private async Task<decimal> ApplyCustomerFeeAsync(int customerId, Restaurant restaurant, decimal totalAmount)
        {
            if (restaurant.FirstFeePercent.HasValue || restaurant.ReturningFeePercent.HasValue)
            {
                var isFirst = await _orderService.CheckIsFirstOrderAsync(customerId, restaurant.Uid);
                if (isFirst && restaurant.FirstFeePercent.HasValue && restaurant.FirstFeePercent.Value > 0)
                {
                    totalAmount *= (1 - (restaurant.FirstFeePercent.Value / 100));
                }
                else if (!isFirst && restaurant.ReturningFeePercent.HasValue && restaurant.ReturningFeePercent.Value > 0)
                {
                    totalAmount *= (1 - (restaurant.ReturningFeePercent.Value / 100));
                }
            }
            return totalAmount;
        }

        // Phương thức thêm danh sách menu vào đơn hàng
        private async Task AddOrderMenusAsync(int orderId, List<OrderMenuModelDto> orderMenus)
        {
            List<CreateOrUpdateOrderMenuDto> createOrUpdateOrderMenuDtos = new List<CreateOrUpdateOrderMenuDto>();
            foreach (var orderMenu in orderMenus)
            {
                var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                if (menu != null)
                {
                    createOrUpdateOrderMenuDtos.Add(new CreateOrUpdateOrderMenuDto
                    {
                        OrderMenuId = 0,
                        OrderId = orderId,
                        MenuId = orderMenu.MenuId,
                        Quantity = orderMenu.Quantity,
                        Price = menu.Price,
                    });
                }
            }
            await _orderMenuService.AddRangeAsync(createOrUpdateOrderMenuDtos);
        }

        // Phương thức gửi email cho đơn hàng
        private async Task SendOrderEmailAsync(int orderId)
        {
            var orderEmail = await _orderService.GetEmailForOrderAsync(orderId, User_Role.RESTAURANT);
            OrderPaidEmail orderPaidEmail = await _orderService.GetOrderPaid(orderId);
            await _sendMailService.SendEmailAsync(orderEmail.Email, Email_Subject.NEWORDER, EmailTemplates.NewOrder(orderPaidEmail));
        }


        // Khi nhà hàng confirm thì khách hàng sẽ chuyển khoản với phương thức thanh toán
        // 1: số dư ví(nếu đủ) 2: VNPAY 3: VIETQR 
        [HttpPost("PaidOrder")]
        [SwaggerOperation(Summary = "Khi nhà hàng confirm thì khách hàng sẽ chuyển khoản với phương thức thanh toán (nếu đơn hàng có giá trị) ISBALANCE | VIETQR | VNPAY: Customer")]
        public async Task<IActionResult> PaidOrder(int orderId, int userId, string paymentGateway)
        {
            // Fetch the order and ensure the user is authorized
            OrderDto order;
            try
            {
                order = await _orderService.GetItemAsync(orderId, userId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }

            // Update order payment status and content
            order.StatusPayment = Payment_Status.PENDING;
            order.ContentPayment = Order_PaymentContent.PaymentContent(order.CustomerId ?? 0, order.RestaurantId ?? 0);

            // Update the order in the system
            var updateOrderResult = await _orderService.UpdatePaidOrderAsync(order);

            if (!updateOrderResult)
            {
                return BadRequest("Cập nhật đơn hàng thất bại.");
            }

            if (paymentGateway.Equals(PaymentGateway.ISBALANCE))
            {
                return await HandleWalletPayment(order);
            }

            if (paymentGateway.Equals(PaymentGateway.VIETQR))
            {
                var orderMenu = await _orderMenuService.GetItemsByOrderAsync(order.OrderId);
                return await HandleVietQRPayment(order, orderMenu, order.TotalAmount);
            }

            if (paymentGateway.Equals(PaymentGateway.VNPAY))
            {
                return await HandleVNPAYPayment(order, order.TotalAmount);
            }

            return BadRequest("Cổng thanh toán không hợp lệ.");
        }

        private async Task<IActionResult> HandleWalletPayment(OrderDto order)
        {
            // Check wallet balance
            var walletBalance = await _walletService.GetBalanceOrderAsync(order.CustomerId ?? 0);

            if (walletBalance < order.TotalAmount)
            {
                return BadRequest("Số dư ví không đủ cho giao dịch này.");
            }

            decimal newWalletBalance = walletBalance - order.TotalAmount;

            // Retrieve user information for wallet transaction
            UserOrderIsBalance userOrderIsBalance;
            try
            {
                userOrderIsBalance = await _userService.GetInformationUserOrderIsBalance(order.CustomerId ?? 0);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            // Create the wallet transaction
            var walletTransactionDto = new WalletTransactionDto
            {
                TransactionId = 0, // Auto-generated
                WalletId = userOrderIsBalance.WalletId,
                Uid = userOrderIsBalance.Id,
                TransactionAmount = order.TotalAmount,
                TransactionType = Transaction_Type.SYSTEMSUBTRACT,
                TransactionDate = DateTime.Now,
                Description = Payment_Descriptions.SystemSubtractDescription(order.TotalAmount),
                Status = Payment_Status.SUCCESSFUL
            };

            // Add the wallet transaction
            var createWalletResult = await _walletTransactionService.AddAsync(walletTransactionDto);
            if (!createWalletResult)
            {
                return BadRequest("Tạo giao dịch ví thất bại.");
            }

            // Update the wallet balance after the transaction
            WalletBalanceOrderDto walletBalanceOrder = new WalletBalanceOrderDto
            {
                Uid = order.CustomerId ?? 0,
                WalletBalance = newWalletBalance
            };

            var updateWalletResult = await _walletService.UpdateWalletBalanceOrderAsync(walletBalanceOrder);

            if (!updateWalletResult)
            {
                return BadRequest("Cập nhật số dư ví thất bại.");
            }

            var updateStatusOrder = await _orderService.UpdateStatusAsync(order.OrderId, Order_Status.PAID);

            if (!updateStatusOrder)
            {
                return BadRequest("Thay đổi trạng thái order thất bại.");
            }


            return Ok("Thanh toán đơn hàng thành công");
        }

        private async Task<IActionResult> HandleVietQRPayment(OrderDto order, List<OrderMenuDto> orderMenuDtos, decimal totalAmount)
        {
            var items = new List<ItemData>();

            if (orderMenuDtos != null && orderMenuDtos.Any())
            {
                foreach (var orderMenu in orderMenuDtos)
                {
                    var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                    if (menu != null && orderMenu.Quantity.HasValue)
                    {
                        items.Add(new ItemData(menu.DishName, (int)orderMenu.Quantity, (int)menu.Price));
                    }
                }
            }

            var paymentData = new PaymentData(
                orderCode: GenerateOrderCodeForVIETQR.GenerateOrderCode(order.OrderId, 26112002),
                amount: (int)totalAmount,
                description: Order_PaymentContent.PaymentContentVIETQR(),
                items: items,
                cancelUrl: _configuration["PaymentSettings:CancelUrl"]+$"&transactionId={order.OrderId}&paymentType=order",
                returnUrl: _configuration["PaymentSettings:CancelUrl"]+$"&transactionId={order.OrderId}&paymentType=order"
            );

            CreatePaymentResult createPayment = await _paymentGatewayService.CreatePaymentUrlPayOS(paymentData);
            return Ok(createPayment.checkoutUrl);
        }

        private async Task<IActionResult> HandleVNPAYPayment(OrderDto order, decimal totalAmount)
        {
            UserPayment userInformation;
            try
            {
                userInformation = await _userService.GetInformationUserToPayment(order.CustomerId ?? 0);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            var paymentInformationModel = new PaymentInformationModel()
            {
                BookingID = order.OrderId.ToString(),
                AccountID = userInformation.Id.ToString(),
                CustomerName = userInformation.Name,
                Amount = (int)totalAmount,
                PaymentType = "Order"
            };

            string linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext);
            return Ok(linkPayment);
        }

        // Khi chuyển khoản xong thì sẽ check status payment của đơn hàng đó
        // có 2 status: 1 Successful (thành công) 2 Cancelled (thất bại)
        [HttpGet("CheckPayment/{orderID}")]
        [SwaggerOperation(Summary = "Khi chuyển khoản xong thì sẽ check status payment của đơn hàng đó Cancelled | Successful: Customer")]
        public async Task<IActionResult> GetItemAsync(int orderID, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            if (status.Equals(Payment_Status.CANCELLED))
            {
                var result = await _orderService.UpdateStatusOrderPayment(orderID, status);
                return result
                    ? Ok(new { message = "Cập nhật trạng thái giao dịch thành công." })
                    : BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
            }

            if (status.Equals(Payment_Status.SUCCESSFUL))
            {
                var result = await _orderService.UpdateStatusOrderPayment(orderID, status);
                // SEND MAIL 
                OrderPaidEmail orderPaidEmail = await _orderService.GetOrderPaid(orderID);

                await _sendMailService.SendEmailAsync(orderPaidEmail.Email, Email_Subject.ORDERCONFIRM, EmailTemplates.Order(orderPaidEmail));

                return result
                        ? Ok(new { message = "Cập nhật trạng thái giao dịch thành công." })
                        : BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
            }
            return BadRequest(new { message = "Trạng thái không hợp lệ. Vui lòng chọn Cancelled hoặc Successful." });
        }


        // xem chi tiết đơn hàng
        [HttpGet("GetOrder/{Uid}/{orderId}")]
        [SwaggerOperation(Summary = "Xem chi tiết đơn hàng: Customer | Restaurant")]
        public async Task<IActionResult> GetItemAsync(int Uid, int orderId)
        {
            try
            {
                var orderDto = await _orderService.GetItemAsync(orderId, Uid);
                var orderTables = await _orderTableService.GetItemsByOrderAsync(orderDto.OrderId);
                var orderMenus = await _orderMenuService.GetItemsByOrderAsync(orderDto.OrderId);
                orderDto.OrderTables = orderTables;
                orderDto.OrderMenus = orderMenus;
                return Ok(orderDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Đơn hàng với ID {orderId} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Bạn không có quyền truy cập vào đơn hàng với ID {orderId}.");
            }
        }

        // List ra đơn hàng của customer
        [HttpGet("GetOrdeHistoryForCustomer/{customerId}")]
        [SwaggerOperation(Summary = "Lấy ra tất cả thông tin đơn hàng của khách hàng: Customer")]
        public async Task<IActionResult> GetCustomerPaging(int pageNumber, int pageSize, int customerId, string? status)
        {
            // Gọi service để lấy dữ liệu có phân trang
            PaginatedList<OrderCustomerDto> result = await _orderService.GetCustomerPagingAsync(pageNumber, pageSize, customerId, status);

            // Tạo response DTO
            var response = new PaginatedResponseDto<OrderCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }


        // List ra đơn hàng của restaurant
        [HttpGet("GetOrderListForRestaurant/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra tất cả thông tin đơn hàng của nhà hàng (có thể search theo status, month, date): Restaurant")]
        public async Task<IActionResult> GetRestaurantPaging(int pageNumber, int pageSize, int restaurantId, string? status, DateTime? month, DateTime? date)
        {
            // Gọi service để lấy dữ liệu có phân trang
            PaginatedList<OrderRestaurantDto> result = await _orderService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date);

            foreach(var item in result)
            {
                item.OrderMenus = await _orderMenuService.GetItemsByOrderAsync(item.OrderId);
                item.OrderTables = await _orderTableService.GetItemsByOrderAsync(item.OrderId);
            }

            // Tạo response DTO
            var response = new PaginatedResponseDto<OrderRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("GetOrderListForAdmin")]
        [SwaggerOperation(Summary = "Lấy ra tất cả thông tin đơn hàng của hệ thống: Admin")]
        public async Task<IActionResult> GetOrderListForAdmin()
        {
            var result = await _orderService.GetAdminPagingAsync();

            foreach (var item in result)
            {
                item.OrderMenus = await _orderMenuService.GetItemsByOrderAsync(item.OrderId);
                item.OrderTables = await _orderTableService.GetItemsByOrderAsync(item.OrderId);
            }

            return Ok(result);
        }


        // Cập nhật trạng thái đơn hàng
        [HttpPut("UpdateStatus/{orderID}")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái đơn hàng (Confirm,Complete): Restaurant")]
        public async Task<IActionResult> UpdateOrderStatus(int orderID, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Trạng thái không thể để trống.");
            }

            if (!status.Equals(Order_Status.CONFIRM) && !status.Equals(Order_Status.COMPLETE))
            {
                return BadRequest("Trạng thái không hợp lệ (Confirm | Complete).");
            }

            var result = await _orderService.UpdateStatusAsync(orderID, status);
            if (result)
            {
                if (status.Equals(Order_Status.COMPLETE))
                {
                    var completeOrder = await _orderService.GetInformationForCompleteAsync(orderID);
                    if (completeOrder != null)
                    {
                        WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
                        {
                            WalletId = completeOrder.WalletId,
                            Uid = completeOrder.RestaurantID,
                            WalletBalance = completeOrder.WalletBalance,
                        };

                        var walletUpdateResult = await _walletService.UpdateWalletBalanceAsync(walletBalanceDto);
                        if (walletUpdateResult)
                        {
                            WalletTransactionDto walletTransactionDto = new WalletTransactionDto()
                            {
                                TransactionId = 0,
                                Uid = completeOrder.RestaurantID,
                                WalletId = completeOrder.WalletId,
                                TransactionAmount = completeOrder.TotalAmount,
                                TransactionType = Transaction_Type.SYSTEMADD,
                                TransactionDate = DateTime.Now,
                                Description = Payment_Descriptions.SystemAddtractDescription(completeOrder.TotalAmount),
                                Status = Payment_Status.SUCCESSFUL,
                            };

                            await _walletTransactionService.AddAsync(walletTransactionDto);
                        }
                    }
                }

                OrderPaidEmail orderPaidEmail = await _orderService.GetOrderPaid(orderID);
                var orderEmail = await _orderService.GetEmailForOrderAsync(orderID, User_Role.CUSTOMER);
                await _sendMailService.SendEmailAsync(orderEmail.Email, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrder(orderPaidEmail, status));

                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {orderID} thành công.");
            }

            return NotFound($"Đơn hàng với ID {orderID} không tồn tại hoặc trạng thái không thay đổi.");
        }

        [HttpPut("CancelOrder")]
        [SwaggerOperation(Summary = "Hủy đơn hàng: Restaurant | Customer")]
        public async Task<IActionResult> CancelOrder(CancelOrderRequest cancelOrderRequest)
        {
            // Cập nhật trạng thái đơn hàng
            var result = await _orderService.UpdateStatusCancelAsync(cancelOrderRequest.OrderId, Order_Status.CANCEL, cancelOrderRequest.CancelReason);

            if (!result)
            {
                return NotFound($"Đơn hàng với ID {cancelOrderRequest.OrderId} không tồn tại hoặc trạng thái không thay đổi.");
            }

            // Lấy thông tin đơn hàng bị hủy
            var cancelOrder = await _orderService.GetInformationForCancelAsync(cancelOrderRequest.UserId, cancelOrderRequest.OrderId);

            if(cancelOrder.TotalAmount <= 0)
            {
                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.");
            }

            var checkStatusOrder = await _orderService.GetItemAsync(cancelOrderRequest.OrderId,cancelOrderRequest.UserId);

            if (checkStatusOrder.PaidAt == null && string.IsNullOrEmpty(checkStatusOrder.PaidAt.ToString()))
            {
                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.");
            }

            // Xử lý tài khoản ví dựa trên vai trò người dùng
            var isCustomer = cancelOrder.RoleName.Equals(User_Role.CUSTOMER);

            if(isCustomer == false)
            {
                await _restaurantService.UpdateReputationScore(cancelOrder.RestaurantID);
            };

            var transactionAmount = isCustomer && cancelOrder.CancellationFeePercent.HasValue && cancelOrder.CancellationFeePercent > 0
                ? cancelOrder.TotalAmount * (1 - (cancelOrder.CancellationFeePercent / 100))
                : cancelOrder.TotalAmount;

            var amountDifference = cancelOrder.TotalAmount - transactionAmount;

            // Kiểm tra nếu khách hàng hủy thì sẽ cộng tiền cho nhà hàng
            if (isCustomer && amountDifference > 0)
            {
                WalletBalanceDto walletBalanceRestaurantDto = new WalletBalanceDto()
                {
                    WalletId = cancelOrder.WalletRestaurantId,
                    Uid = cancelOrder.RestaurantID,
                    WalletBalance = cancelOrder.WalletBalanceRestaurant + amountDifference
                };

                var walletUpdateRestaurantResult = await _walletService.UpdateWalletBalanceAsync(walletBalanceRestaurantDto);
                if (walletUpdateRestaurantResult)
                {
                    WalletTransactionDto walletTransactionRestaurantDto = new WalletTransactionDto()
                    {
                        TransactionId = 0,
                        Uid = cancelOrder.RestaurantID,
                        WalletId = cancelOrder.WalletRestaurantId,
                        TransactionAmount = amountDifference ?? 0,
                        TransactionType = Transaction_Type.SYSTEMADD,
                        TransactionDate = DateTime.Now,
                        Description = Payment_Descriptions.SystemAddtractDescription(amountDifference ?? 0),
                        Status = Payment_Status.SUCCESSFUL,
                    };
                    await _walletTransactionService.AddAsync(walletTransactionRestaurantDto);
                }
            };

            WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
            {
                WalletId = cancelOrder.WalletCustomerId,
                Uid = cancelOrder.CustomerID,
                WalletBalance = cancelOrder.WalletBalanceCustomer + transactionAmount
            };

            // Cập nhật số dư ví
            var walletUpdateResult = await _walletService.UpdateWalletBalanceAsync(walletBalanceDto);

            if (walletUpdateResult)
            {
                // Ghi lại giao dịch
                WalletTransactionDto walletTransactionDto = new WalletTransactionDto()
                {
                    TransactionId = 0,
                    Uid = cancelOrder.CustomerID,
                    WalletId = cancelOrder.WalletCustomerId,
                    TransactionAmount = transactionAmount ?? 0,
                    TransactionType = Transaction_Type.SYSTEMADD,
                    TransactionDate = DateTime.Now,
                    Description = Payment_Descriptions.SystemAddtractDescription(transactionAmount ?? 0),
                    Status = Payment_Status.SUCCESSFUL,
                };
                await _walletTransactionService.AddAsync(walletTransactionDto);
            }

            // Gửi email thông báo
            var recipientEmail = isCustomer ? cancelOrder.EmailRestaurant : cancelOrder.EmailCustomer;

            OrderPaidEmail orderPaidEmail = await _orderService.GetOrderPaid(cancelOrderRequest.OrderId);
            if (isCustomer == true)
            {
                await _sendMailService.SendEmailAsync(recipientEmail, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrder(orderPaidEmail, Order_Status.CANCEL));
            }
            else
            {
                await _sendMailService.SendEmailAsync(recipientEmail, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrderRestaurant(orderPaidEmail, Order_Status.CANCEL));
            }
            return Ok($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.");
        }

        [HttpPut("UpdateMultiStatusOrders")]
        [SwaggerOperation(Summary = "Cập nhật một loạt trạng thái đơn hàng Pending: Restaurant")]
        public async Task<IActionResult> AddTablesToOrder([FromBody] MultiStatusOrders statusOrders)
        {
            if (statusOrders == null || !statusOrders.OrderID.Any())
            {
                return BadRequest("Yêu cầu không hợp lệ: Cần có thông tin đơn hàng và id đơn hàng.");
            }

            var result = await _orderService.UpdatePendingOrdersAsync(statusOrders);

            if (result)
            {
                return Ok("Cập nhật thành công.");
            }
            else
            {
                return StatusCode(500, "Cập nhật thất bại.");
            }
        }

        // ORDER TABLE 
        [HttpPost("CreateOrderTable")]
        [SwaggerOperation(Summary = "Tạo order table cho đơn hàng Nếu khách hàng không chọn được bàn: Restaurant")]
        public async Task<IActionResult> AddTablesToOrder([FromBody] CreateRestaurantOrderTablesDto orderTablesDto)
        {
            if (orderTablesDto == null || !orderTablesDto.TableIds.Any())
            {
                return BadRequest("Yêu cầu không hợp lệ: Cần có thông tin đơn hàng và bàn.");
            }

            var result = await _orderTableService.AddRangeAsync(orderTablesDto);
            if (result)
            {
                return Ok("Các bàn đã được thêm thành công vào đơn hàng.");
            }
            else
            {
                return StatusCode(500, "Không thể thêm bàn vào đơn hàng.");
            }
        }

        [HttpGet("GetOrderTable/{orderId}")]
        [SwaggerOperation(Summary = "Lấy danh sách order table cho đơn hàng: Restaurant | Customer")]
        public async Task<IActionResult> GetTableItemsByOrder(int orderId)
        {
            var orderTables = await _orderTableService.GetItemsByOrderAsync(orderId);

            if (orderTables == null || !orderTables.Any())
            {
                return NotFound("Không tìm thấy bàn cho đơn hàng này.");
            }
            return Ok(orderTables);
        }

        //// ORDER MENU 
        [HttpGet("GetOrderMenu/{orderId}")]
        [SwaggerOperation(Summary = "Lấy danh sách order menu cho đơn hàng: Restaurant | Customer")]
        public async Task<IActionResult> GetMenuItemsByOrderAsync(int orderId)
        {
            var items = await _orderMenuService.GetItemsByOrderAsync(orderId);
            if (items == null || !items.Any())
            {
                return NotFound("Không tìm thấy món cho đơn hàng này.");
            }
            return Ok(items);
        }

        [HttpPut("ChangeMenus")]
        public async Task<IActionResult> ChangeMenusAsync(ChangeOrderMenuDto changeOrderMenu)
        {
            if (changeOrderMenu.orderMenus == null || !changeOrderMenu.orderMenus.Any())
            {
                return BadRequest("Menu list cannot be empty.");
            }

            List<CreateOrUpdateOrderMenuDto> createOrUpdateOrderMenuDtos = new List<CreateOrUpdateOrderMenuDto>();
            foreach (var orderMenu in changeOrderMenu.orderMenus)
            {
                var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                if (menu != null)
                {
                    createOrUpdateOrderMenuDtos.Add(new CreateOrUpdateOrderMenuDto
                    {
                        OrderMenuId = 0, // Bạn có thể cần thay đổi OrderMenuId nếu đang cập nhật
                        OrderId = changeOrderMenu.OrderId,
                        MenuId = orderMenu.MenuId,
                        Quantity = orderMenu.Quantity,
                        Price = menu.Price,
                    });
                }
            }

            if (createOrUpdateOrderMenuDtos.Count == 0)
            {
                return BadRequest("No valid menus found for the order.");
            }

            var result = await _orderMenuService.ChangeMenusAsync(changeOrderMenu.OrderId, createOrUpdateOrderMenuDtos);

            if (!result)
            {
                return BadRequest("Failed to change menus.");
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(changeOrderMenu.RestaurantId);
            if (restaurant == null)
            {
                return NotFound("Restaurant does not exist.");
            }

            decimal totalAmount = 0;

            if (restaurant.Price > 0)
            {
                totalAmount += restaurant.Price;

                if (restaurant.Discount.HasValue && restaurant.Discount > 0)
                {
                    totalAmount *= (1 - (restaurant.Discount.Value / 100m));
                }
            }

            CaculatorOrderDto caculatorOrder = new CaculatorOrderDto()
            {
                CustomerId = changeOrderMenu.CustomerId,
                RestaurantId = changeOrderMenu.RestaurantId,
                OrderMenus = changeOrderMenu.orderMenus
            };

            // Tính tổng tiền từ thực đơn
            totalAmount += await CalculateMenuTotalForPreOrderAsync(caculatorOrder);

            // Áp dụng phí dựa trên trạng thái khách hàng
            totalAmount = await ApplyCustomerFeeAsync(caculatorOrder.CustomerId, restaurant, totalAmount);

            var resultUpdateOrder = await _orderService.UpdateTotalIncomeChangeMenuAsync(changeOrderMenu.OrderId, totalAmount);

            if (resultUpdateOrder)
            {
                return Ok("Successfully changed menus and updated the order.");
            }

            return BadRequest("Failed to update the order!");
        }

    }
}
