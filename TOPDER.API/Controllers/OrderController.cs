using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Org.BouncyCastle.Utilities.Encoders;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.BlogGroup;
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
        private readonly IUserService _userService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ISendMailService _sendMailService;


        public OrderController(IOrderService orderService, IOrderMenuService orderMenuService,
            IWalletService walletService, IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository, IDiscountRepository discountRepository,
            IUserService userService, IWalletTransactionService walletTransactionService,
            IPaymentGatewayService paymentGatewayService, ISendMailService sendMailService, IOrderTableService orderTableService)
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
        }


        // ORDER

        // Tạo đơn hàng
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return BadRequest("Order cannot be null.");
            }

            decimal totalAmount = 0;
            var restaurant = await _restaurantRepository.GetByIdAsync(orderModel.RestaurantId);

            // Calculate total amount from OrderMenus
            if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
            {
                foreach (var orderMenu in orderModel.OrderMenus)
                {
                    var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                    if (menu != null)
                    {
                        totalAmount += menu.Price * (orderMenu.Quantity ?? 1);
                    }
                }
            }

            // Apply restaurant discount
            if (restaurant.Price > 0 && restaurant.Discount > 0)
            {
                totalAmount += restaurant.Price * (1 - (restaurant.Discount.Value / 100));
            }

            // Check and apply order discount
            if (orderModel.DiscountId.HasValue && orderModel.DiscountId.Value != 0)
            {
                var discount = await _discountRepository.GetByIdAsync(orderModel.DiscountId.Value);
                if (discount != null && discount.IsActive == true && discount.Quantity > 0)
                {
                    totalAmount *= (1 - (discount.DiscountPercentage / 100));
                    discount.Quantity -= 1;
                    await _discountRepository.UpdateAsync(discount);
                }
            }

            // Apply fee percentages based on customer status
            if (restaurant.FirstFeePercent.HasValue || restaurant.ReturningFeePercent.HasValue)
            {
                var isFirst = await _orderService.CheckIsFirstOrderAsync(orderModel.CustomerId, orderModel.RestaurantId);
                if (isFirst && restaurant.FirstFeePercent.HasValue && restaurant.FirstFeePercent.Value > 0)
                {
                    totalAmount *= (1 - (restaurant.FirstFeePercent.Value / 100));
                }
                else if (!isFirst && restaurant.ReturningFeePercent.HasValue && restaurant.ReturningFeePercent.Value > 0)
                {
                    totalAmount *= (1 - (restaurant.ReturningFeePercent.Value / 100));
                }
            }

            // Create Order
            OrderDto orderDto = new OrderDto
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
                if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
                {
                    List<CreateOrUpdateOrderMenuDto> createOrUpdateOrderMenuDtos = new List<CreateOrUpdateOrderMenuDto>();
                    foreach (var orderMenu in orderModel.OrderMenus)
                    {
                        var menu = await _menuRepository.GetByIdAsync(orderMenu.MenuId);
                        if (menu != null)
                        {
                            createOrUpdateOrderMenuDtos.Add(new CreateOrUpdateOrderMenuDto
                            {
                                OrderMenuId = 0,
                                OrderId = order.OrderId,
                                MenuId = orderMenu.MenuId,
                                Quantity = orderMenu.Quantity,
                                Price = menu.Price,
                            });
                        }
                    }
                    await _orderMenuService.AddRangeAsync(createOrUpdateOrderMenuDtos);
                }
                var orderEmail = await _orderService.GetEmailForOrderAsync(order.OrderId, User_Role.RESTAURANT);
                await _sendMailService.SendEmailAsync(orderEmail.Email,Email_Subject.NEWORDER, EmailTemplates.NewOrder(orderEmail.Name,orderEmail.OrderId));
            }
            return BadRequest("Tạo đơn hàng thất bại");
        }



        // Khi nhà hàng confirm thì khách hàng sẽ chuyển khoản với phương thức thanh toán
        // 1: số dư ví(nếu đủ) 2: VNPAY 3: VIETQR 
        [HttpPost("PaidOrder/{orderId}/{userId}/{paymentGateway}")]
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
            var updateOrderResult = await _orderService.UpdateAsync(order);

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
                Description = Payment_Descriptions.SystemSubtractDescription(userOrderIsBalance.Name, userOrderIsBalance.Id),
                Status = Payment_Status.PENDING
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

            return Ok("Tạo đơn hàng thành công");
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
                orderCode: order.OrderId,
                amount: (int)totalAmount,
                description: Order_PaymentContent.PaymentContent(order.CustomerId ?? 0, order.RestaurantId ?? 0),
                items: items,
                cancelUrl: "https://yourapp.com/cancel",
                returnUrl: "https://yourapp.com/return"
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
            };

            string linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext);
            return Ok(linkPayment);
        }

        // Khi chuyển khoản xong thì sẽ check status payment của đơn hàng đó
        // có 2 status: 1 Successful (thành công) 2 Cancelled (thất bại)
        [HttpGet("CheckPayment/{orderID}")]
        public async Task<IActionResult> GetItemAsync(int orderID, [FromBody] string status)
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemAsync(int id, int Uid)
        {
            try
            {
                var orderDto = await _orderService.GetItemAsync(id, Uid);
                return Ok(orderDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Đơn hàng với ID {id} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Bạn không có quyền truy cập vào đơn hàng với ID {id}.");
            }
        }

        // List ra đơn hàng của customer
        [HttpGet("Customer/List/{customerId}")]
        public async Task<IActionResult> GetCustomerPaging(int pageNumber, int pageSize, int customerId, string? status)
        {
            // Gọi service để lấy dữ liệu có phân trang
            var result = await _orderService.GetCustomerPagingAsync(pageNumber, pageSize, customerId, status);

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
        [HttpGet("Restaurant/List/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantPaging(int pageNumber, int pageSize, int restaurantId, string? status, DateTime? month, DateTime? date)
        {
            // Gọi service để lấy dữ liệu có phân trang
            var result = await _orderService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date);

            // Tạo response DTO
            var response = new PaginatedResponseDto<OrderDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }


        // cập nhật đơn hàng
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Đơn hàng không thể là null.");
            }

            var result = await _orderService.UpdateAsync(orderDto);
            if (result)
            {
                return Ok($"Cập nhật đơn hàng với ID {orderDto.OrderId} thành công."); // Trả về thông điệp thành công
            }

            return NotFound($"Đơn hàng với ID {orderDto.OrderId} không tồn tại."); // Thông báo không tìm thấy
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPut("UpdateStatus/{orderID}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderID, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Trạng thái không thể để trống."); // Kiểm tra trạng thái không null hoặc empty
            }

            if (!status.Equals(Order_Status.PENDING) && !status.Equals(Order_Status.CONFIRM) && !status.Equals(Order_Status.PAID)
                && !status.Equals(Order_Status.COMPLETE) && !status.Equals(Order_Status.CANCEL))
            {
                return BadRequest("Trạng thái không hợp lệ (Pending | Confirm | Paid | Complete | Cancel) ");
            }

            var result = await _orderService.UpdateStatusAsync(orderID, status);
            if (result)
            {
                if (status.Equals(Order_Status.CONFIRM) || status.Equals(Order_Status.COMPLETE) || status.Equals(Order_Status.CANCEL))
                {       
                    var orderEmail = await _orderService.GetEmailForOrderAsync(orderID, User_Role.CUSTOMER);
                    await _sendMailService.SendEmailAsync(orderEmail.Email,Email_Subject.UPDATESTATUS, EmailTemplates.OrderStatusUpdate(orderEmail.Name, orderEmail.OrderId, status));
                }
                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {orderID} thành công."); // Trả về thông điệp thành công
            }

            return NotFound($"Đơn hàng với ID {orderID} không tồn tại hoặc trạng thái không thay đổi."); // Thông báo không tìm thấy
        }




        //[HttpDelete("{id}")]
        //public async Task<IActionResult> RemoveOrder(int id)
        //{
        //    var result = await _orderService.RemoveAsync(id);
        //    if (result)
        //    {
        //        return Ok($"Xóa đơn hàng với ID {id} thành công."); // Trả về thông điệp thành công
        //    }

        //    return NotFound($"Đơn hàng với ID {id} không tồn tại."); // Thông báo không tìm thấy
        //}


        // ORDER TABLE 

        [HttpPost("Table/Create")]
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

        [HttpGet("Table/{id}")]
        public async Task<IActionResult> GetTableItemsByOrder(int id)
        {
            var orderTables = await _orderTableService.GetItemsByOrderAsync(id);

            if (orderTables == null || !orderTables.Any())
            {
                return NotFound("Không tìm thấy bàn cho đơn hàng này.");
            }
            return Ok(orderTables);
        }

        // ORDER MENU 

        [HttpGet("Menu/{id}")]
        public async Task<IActionResult> GetMenuItemsByOrderAsync(int id)
        {
            var items = await _orderMenuService.GetItemsByOrderAsync(id);
            if (items == null || !items.Any())
            {
                return NotFound("Không tìm thấy món cho đơn hàng này.");
            }
            return Ok(items);
        }

    }
}
