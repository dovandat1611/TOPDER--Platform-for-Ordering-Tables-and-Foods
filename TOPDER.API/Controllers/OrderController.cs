using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
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



        public OrderController(IOrderService orderService, IOrderMenuService orderMenuService,
            IWalletService walletService, IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository, IDiscountRepository discountRepository,
            IUserService userService, IWalletTransactionService walletTransactionService, IPaymentGatewayService paymentGatewayService)
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

           
            if (orderModel.IsBalance == true)
            {
                var walletBalance = await _walletService.GetBalanceOrderAsync(orderModel.CustomerId);
                if (walletBalance >= totalAmount)
                {
                    decimal newWalletBalance = walletBalance - totalAmount;

                    // Create order
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
                        StatusPayment = Payment_Status.PENDING,
                        StatusOrder = Order_Status.PENDING,
                        ContentPayment = Order_PaymentContent.PaymentContent(orderModel.CustomerId, orderModel.RestaurantId),
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

                        // Create wallet transaction
                        UserOrderIsBalance userOrderIsBalance;
                        try
                        {
                            userOrderIsBalance = await _userService.GetInformationUserOrderIsBalance(orderModel.CustomerId);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            return NotFound(new { message = ex.Message });
                        }

                        var walletTransactionDto = new WalletTransactionDto
                        {
                            TransactionId = 0,
                            WalletId = userOrderIsBalance.WalletId,
                            Uid = userOrderIsBalance.Id,
                            TransactionAmount = totalAmount,
                            TransactionType = Transaction_Type.SYSTEMSUBTRACT,
                            TransactionDate = DateTime.Now,
                            Description = Payment_Descriptions.SystemSubtractDescription(userOrderIsBalance.Name, userOrderIsBalance.Id),
                            Status = Payment_Status.PENDING
                        };

                        // Create walletTransaction
                        var createWallet = await _walletTransactionService.AddAsync(walletTransactionDto);
                        if (createWallet)
                        {
                            WalletBalanceOrderDto walletBalanceOrder = new WalletBalanceOrderDto
                            {
                                Uid = orderModel.CustomerId,
                                WalletBalance = newWalletBalance
                            };
                            await _walletService.UpdateWalletBalanceOrderAsync(walletBalanceOrder);
                        }
                        return Ok("Tạo đơn hàng thành công");
                    }
                }
                else
                {
                    return BadRequest("Số dư ví không đủ cho giao dịch này.");
                }
            }

            // Handle other payment gateways (VietQR and VNPay)
            if (!string.IsNullOrEmpty(orderModel.PaymentGateway))
            {
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
                    StatusPayment = Payment_Status.PENDING,
                    StatusOrder = Order_Status.PENDING,
                    ContentPayment = Order_PaymentContent.PaymentContent(orderModel.CustomerId, orderModel.RestaurantId),
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

                    if (orderModel.PaymentGateway.Equals(PaymentGateway.VIETQR))
                    {
                        return await HandleVietQRPayment(order, orderModel, totalAmount);
                    }
                    else if (orderModel.PaymentGateway.Equals(PaymentGateway.VNPAY))
                    {
                        return await HandleVNPAYPayment(order, orderModel, totalAmount);
                    }
                }
            }

            return BadRequest("Tạo đơn hàng thất bại");
        }

        private async Task<IActionResult> HandleVietQRPayment(Order order, OrderModel orderModel, decimal totalAmount)
        {
            var items = new List<ItemData>();

            if (orderModel.OrderMenus != null && orderModel.OrderMenus.Any())
            {
                foreach (var orderMenu in orderModel.OrderMenus)
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
                description: Order_PaymentContent.PaymentContent(orderModel.CustomerId, orderModel.RestaurantId),
                items: items,
                cancelUrl: "https://yourapp.com/cancel",
                returnUrl: "https://yourapp.com/return"
            );

            CreatePaymentResult createPayment = await _paymentGatewayService.CreatePaymentUrlPayOS(paymentData);
            return Ok(createPayment.checkoutUrl);
        }


        private async Task<IActionResult> HandleVNPAYPayment(Order order, OrderModel orderModel, decimal totalAmount)
        {
            UserPayment userInformation;
            try
            {
                userInformation = await _userService.GetInformationUserToPayment(orderModel.CustomerId);
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


        /// <summary>
        /// Gets an order by ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="Uid">The user ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the order DTO.</returns>
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetItemAsync(int id, int Uid)
        //{
        //    try
        //    {
        //        var orderDto = await _orderService.GetItemAsync(id, Uid);
        //        return Ok(orderDto);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound($"Order with id {id} not found.");
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Forbid($"You do not have access to order with id {id}.");
        //    }
        //}

        /// <summary>
        /// Gets paginated orders for a specific customer.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of order DTOs.</returns>
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
