using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Org.BouncyCastle.Utilities.Encoders;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
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
            IPaymentGatewayService paymentGatewayService, ISendMailService sendMailService)
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
        }

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
                    await _orderMenuService.AddAsync(createOrUpdateOrderMenuDtos);
                }
            }

            return BadRequest("Tạo đơn hàng thất bại");
        }



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
            return BadRequest(new { message = "Trạng thái không hợp lệ. Vui lòng chọn CANCELLED hoặc SUCCESSFUL." });
        }

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
                return NotFound($"Order with id {id} not found.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"You do not have access to order with id {id}.");
            }
        }

    
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerPaging(int pageNumber, int pageSize, int customerId)
        {
            var paginatedOrders = await _orderService.GetCustomerPagingAsync(pageNumber, pageSize, customerId);
            return Ok(paginatedOrders);
        }

        /// <summary>
        /// Gets paginated orders for a specific restaurant.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="restaurantId">The restaurant ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of order DTOs.</returns>
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantPaging(int pageNumber, int pageSize, int restaurantId)
        {
            var paginatedOrders = await _orderService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId);
            return Ok(paginatedOrders);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="orderDto">The order DTO with updated information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an action result indicating success or failure.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order cannot be null.");
            }

            var result = await _orderService.UpdateAsync(orderDto);
            if (result)
            {
                return NoContent(); // Success, but no content to return
            }

            return NotFound($"Order with ID {orderDto.OrderId} not found.");
        }

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an action result indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOrder(int id)
        {
            // Implement the remove functionality
            // Assuming RemoveAsync is implemented in your service
            var result = await _orderService.RemoveAsync(id);
            if (result)
            {
                return NoContent(); // Success
            }

            return NotFound($"Order with ID {id} not found.");
        }
    }
}
