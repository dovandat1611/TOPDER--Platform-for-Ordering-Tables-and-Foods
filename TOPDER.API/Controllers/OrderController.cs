using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Hubs;
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
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _signalRHub;
        private readonly IRestaurantPolicyService _restaurantPolicyService;
        private readonly IOrderRepository _orderRepository;



        public OrderController(IOrderService orderService, IOrderMenuService orderMenuService,
            IWalletService walletService, IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository, IDiscountRepository discountRepository,
            IUserService userService, IWalletTransactionService walletTransactionService,
            IPaymentGatewayService paymentGatewayService, ISendMailService sendMailService,
            IOrderTableService orderTableService, IDiscountMenuRepository discountMenuRepository,
            IConfiguration configuration, IRestaurantService restaurantService,
            INotificationService notificationService,
            IHubContext<AppHub> signalRHub, IRestaurantPolicyService restaurantPolicyService, IOrderRepository orderRepository)
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
            _notificationService = notificationService;
            _signalRHub = signalRHub;
            _restaurantPolicyService = restaurantPolicyService;
            _orderRepository = orderRepository;
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

            decimal foodAmount = 0;
            decimal depositAmount = 0;

            if (restaurant.Price > 0)
            {
                depositAmount += restaurant.Price;

                if (restaurant.Discount.HasValue && restaurant.Discount > 0)
                {
                    depositAmount *= (1 - (restaurant.Discount.Value / 100m));
                }
            }

            foodAmount += await CalculateMenuTotalForPreOrderAsync(caculatorOrder);

           // depositAmount = await ApplyCustomerFeeAsync(caculatorOrder.CustomerId, restaurant, depositAmount);

           // foodAmount = await ApplyCustomerFeeAsync(caculatorOrder.CustomerId, restaurant, foodAmount);

            CaculatorAmountRespone caculatorAmount = new CaculatorAmountRespone()
            {
                DepositAmount = depositAmount,
                FoodAmount = foodAmount
            };

            return Ok(caculatorAmount);
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

                    NotificationDto notificationResDto = new NotificationDto()
                    {
                        NotificationId = 0,
                        Uid = orderModel.RestaurantId,
                        CreatedAt = DateTime.Now,
                        Content = Notification_Content.ORDER_CREATE(0,0),
                        Type = Notification_Type.ORDER,
                        IsRead = false,
                    };

                    NotificationDto notificationCusDto = new NotificationDto()
                    {
                        NotificationId = 0,
                        Uid = orderModel.CustomerId,
                        CreatedAt = DateTime.Now,
                        Content = Notification_Content.ORDER_CREATE_CUS(0, 0),
                        Type = Notification_Type.ORDER,
                        IsRead = false,
                    };

                    var notificationRes = await _notificationService.AddAsync(notificationResDto);

                    var notificationCus = await _notificationService.AddAsync(notificationCusDto);

                    if (notificationRes != null && notificationCus != null)
                    {
                        List<NotificationDto> notificationDto = new List<NotificationDto> { notificationRes, notificationCus };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notificationDto);
                    }

                    return Ok("Tạo đơn hàng miễn phí thành công");
                }
                return BadRequest("Tạo đơn hàng miễn phí thất bại");
            }


            // Tính toán từng tiền cho đơn hàng
            CaculatorAmountRespone caculatorAmount = await CalculateTotalAmountAsync(orderModel, restaurant);

            // Tạo đơn hàng
            var orderDto = new OrderDto
            {
                OrderId = 0,
                CustomerId = orderModel.CustomerId,
                RestaurantId = orderModel.RestaurantId,
                DiscountId = orderModel.DiscountId ?? null,
                NameReceiver = orderModel.NameReceiver,
                PhoneReceiver = orderModel.PhoneReceiver,
                TimeReservation = orderModel.TimeReservation,
                DateReservation = orderModel.DateReservation,
                NumberPerson = orderModel.NumberPerson,
                NumberChild = orderModel.NumberChild,
                ContentReservation = orderModel.ContentReservation,
                TypeOrder = Order_Type.RESERVATION,
                DepositAmount = caculatorAmount.DepositAmount,
                FoodAmount = caculatorAmount.FoodAmount,
                TotalAmount = (caculatorAmount.FoodAmount + caculatorAmount.DepositAmount),
                TotalPaymentAmount = null,
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

                NotificationDto notificationResDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = orderModel.RestaurantId,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.ORDER_CREATE(orderDto.FoodAmount ?? 0, orderDto.DepositAmount ?? 0),
                    Type = Notification_Type.ORDER,
                    IsRead = false,
                };

                NotificationDto notificationCusDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = orderModel.CustomerId,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.ORDER_CREATE_CUS(orderDto.FoodAmount ?? 0, orderDto.DepositAmount ?? 0),
                    Type = Notification_Type.ORDER,
                    IsRead = false,
                };

                var notificationRes = await _notificationService.AddAsync(notificationResDto);

                var notificationCus = await _notificationService.AddAsync(notificationCusDto);


                if (notificationRes != null && notificationCus != null)
                {
                    List<NotificationDto> notificationDto = new List<NotificationDto> { notificationRes, notificationCus };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notificationDto);
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
                NameReceiver = orderModel.NameReceiver,
                PhoneReceiver = orderModel.PhoneReceiver,
                TimeReservation = orderModel.TimeReservation,
                DateReservation = orderModel.DateReservation,
                NumberPerson = orderModel.NumberPerson,
                NumberChild = orderModel.NumberChild,
                ContentReservation = orderModel.ContentReservation,
                TypeOrder = Order_Type.RESERVATION,
                FoodAmount = 0,
                DepositAmount = 0,
                TotalAmount = 0,
                TotalPaymentAmount = null,
                StatusOrder = Order_Status.PENDING,
                CreatedAt = DateTime.Now
            };

            return await _orderService.AddAsync(orderDtoToFree);
        }

        // Phương thức tính toán tổng tiền đơn hàng
        private async Task<CaculatorAmountRespone> CalculateTotalAmountAsync(OrderModel orderModel, Restaurant restaurant)
        {
            decimal foodAmount = 0;
            decimal depositAmount = 0;

            // Kiểm tra và áp dụng giảm giá cho nhà hàng
            if (restaurant.Price > 0)
            {
                depositAmount += restaurant.Price;
                if (restaurant.Discount > 0)
                {
                    depositAmount *= (1 - (restaurant.Discount.Value / 100));
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
                        foodAmount += await CalculateMenuTotalAsync(orderModel);
                        foodAmount *= (1 - (discount.DiscountPercentage / 100)) ?? 0;
                        discount.Quantity -= 1;
                        await _discountRepository.UpdateAsync(discount);
                    }
                    else if (discount.Scope == DiscountScope.PER_SERVICE)
                    {
                        foodAmount = await ApplyServiceDiscountAsync(orderModel, discount, foodAmount);
                    }
                }
            }
            else
            {
                foodAmount += await CalculateMenuTotalAsync(orderModel);
            }

            //depositAmount = await ApplyCustomerFeeAsync(orderModel.CustomerId, restaurant, depositAmount);

            CaculatorAmountRespone caculatorAmount = new CaculatorAmountRespone()
            {
                DepositAmount = depositAmount,
                FoodAmount = foodAmount,
            };

            return caculatorAmount;
        }

        private async Task<decimal> CalculateTotalAmountForDiscountAsync(OrderModel orderModel)
        {
            decimal totalAmount = 0;

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
            var restaurantPolicy = await _restaurantPolicyService.GetActivePolicyAsync(restaurant.Uid);

            if(restaurantPolicy != null) {
                if (restaurantPolicy.FirstFeePercent != 0 || restaurantPolicy.ReturningFeePercent != 0)
                {
                    var isFirst = await _orderService.CheckIsFirstOrderAsync(customerId, restaurant.Uid);
                    if (isFirst && restaurantPolicy.FirstFeePercent.HasValue && restaurantPolicy.FirstFeePercent.Value > 0)
                    {
                        totalAmount *= (1 - (restaurantPolicy.FirstFeePercent.Value / 100));
                    }
                    else if (!isFirst && restaurantPolicy.ReturningFeePercent.HasValue && restaurantPolicy.ReturningFeePercent.Value > 0)
                    {
                        totalAmount *= (1 - (restaurantPolicy.ReturningFeePercent.Value / 100));
                    }
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
                        OrderMenuType = OrderMenu_Type.ORIGINAL
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
        public async Task<IActionResult> PaidOrder(int orderId, int userId,  string paymentGateway, string typeOrder)
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

            if(!typeOrder.Equals(Paid_Type.DEPOSIT)  && !typeOrder.Equals(Paid_Type.ENTIRE_ORDER))
            {
                return BadRequest(new { message = "Chọn typeOrder là Deposit hoặc EntireOrder!" });
            }

            // lấy thông tin nhà hàng 
            var restaurant = await _restaurantRepository.GetByIdAsync(order.RestaurantId ?? 0);
            decimal totalPaymentAmount = 0;
            if (typeOrder == Paid_Type.DEPOSIT)
            {
                totalPaymentAmount = await ApplyCustomerFeeAsync(order.CustomerId ?? 0, restaurant, order.DepositAmount ?? 0);

            }
            if (typeOrder == Paid_Type.ENTIRE_ORDER)
            {
                totalPaymentAmount = await ApplyCustomerFeeAsync(order.CustomerId ?? 0, restaurant, order.TotalAmount ?? 0);
            }
            // Update order payment status and content
            order.StatusPayment = Payment_Status.PENDING;
            order.ContentPayment = Order_PaymentContent.PaymentContent(order.CustomerId ?? 0, order.RestaurantId ?? 0);
            order.PaidType = typeOrder;
            order.TotalPaymentAmount = totalPaymentAmount;

            // Update the order in the system
            var updateOrderResult = await _orderService.UpdatePaidOrderAsync(order);

            if (!updateOrderResult)
            {
                return BadRequest("Cập nhật đơn hàng thất bại.");
            }

            decimal totalAmount = 0;
            if (typeOrder == Paid_Type.DEPOSIT)
            {
                totalAmount = totalPaymentAmount;
                
            }
            if (typeOrder == Paid_Type.ENTIRE_ORDER)
            {
                totalAmount = totalPaymentAmount; 
            }

            if (paymentGateway.Equals(PaymentGateway.ISBALANCE))
            {
                return await HandleWalletPayment(order, totalAmount);
            }

            if (paymentGateway.Equals(PaymentGateway.VIETQR))
            {
                List<OrderMenuDto> orderMenu = null;
                if (typeOrder == Paid_Type.ENTIRE_ORDER)
                {
                    orderMenu = await _orderMenuService.GetItemsOriginalByOrderAsync(order.OrderId);
                }
                return await HandleVietQRPayment(order, orderMenu, totalAmount);
            }

            if (paymentGateway.Equals(PaymentGateway.VNPAY))
            {
                return await HandleVNPAYPayment(order, totalAmount);
            }

            return BadRequest("Cổng thanh toán không hợp lệ.");
        }

        private async Task<IActionResult> HandleWalletPayment(OrderDto order, decimal totalAmount)
        {
            // Check wallet balance
            var walletBalance = await _walletService.GetBalanceOrderAsync(order.CustomerId ?? 0);

            if (walletBalance < totalAmount)
            {
                return BadRequest("Số dư ví không đủ cho giao dịch này.");
            }

            decimal newWalletBalance = walletBalance - totalAmount;

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
                TransactionAmount = totalAmount,
                TransactionType = Transaction_Type.SYSTEMSUBTRACT,
                TransactionDate = DateTime.Now,
                Description = Payment_Descriptions.SystemSubtractDescription(totalAmount),
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

            if (updateStatusOrder == null)
            {
                return BadRequest("Thay đổi trạng thái order thất bại.");
            }

            // NOTI
            NotificationDto notificationCusDto = new NotificationDto()
            {
                NotificationId = 0,
                Uid = updateStatusOrder.CustomerId ?? 0,
                CreatedAt = DateTime.Now,
                Content = Notification_Content.ORDER_PAYMENT_WALLET(totalAmount),
                Type = Notification_Type.ORDER,
                IsRead = false,
            };

            NotificationDto notificationResDto = new NotificationDto()
            {
                NotificationId = 0,
                Uid = updateStatusOrder.RestaurantId ?? 0,
                CreatedAt = DateTime.Now,
                Content = Notification_Content.ORDER_PAYMENT_WALLET(totalAmount),
                Type = Notification_Type.ORDER,
                IsRead = false,
            };

            NotificationDto notificationWalletResDto = new NotificationDto()
            {
                NotificationId = 0,
                Uid = updateStatusOrder.CustomerId ?? 0,
                CreatedAt = DateTime.Now,
                Content = Notification_Content.SYSTEM_SUB(totalAmount),
                Type = Notification_Type.SYSTEM_SUB,
                IsRead = false,
            };

            var notificationCus = await _notificationService.AddAsync(notificationCusDto);
            var notificationRes = await _notificationService.AddAsync(notificationResDto);
            var notificationWalletRes = await _notificationService.AddAsync(notificationWalletResDto);


            if (notificationCus != null && notificationRes != null && notificationWalletRes != null)
            {
                List<NotificationDto> notifications = new List<NotificationDto> { notificationCus, notificationRes, notificationWalletRes };
                await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
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
                orderCode: GenerateOrderCodeForVIETQR.GenerateOrderCode(order.OrderId, VIETQR_SetupId.ORDER),
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

            string linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext, VNPAY_TypePayment.ORDER);
            return Ok(linkPayment);
        }

        // Khi chuyển khoản xong thì sẽ check status payment của đơn hàng đó
        // có 2 status: 1 Successful (thành công) 2 Cancelled (thất bại)
        [HttpGet("CheckPayment/{orderID}")]
        [SwaggerOperation(Summary = "Khi chuyển khoản xong thì sẽ check status payment của đơn hàng đó Cancelled | Successful: Customer")]
        public async Task<IActionResult> CheckPayment(int orderID, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            var order = await _orderRepository.GetByIdAsync(orderID);
            if(order != null)
            {
                if (order.StatusPayment.Equals(Payment_Status.CANCELLED) || order.StatusPayment.Equals(Payment_Status.SUCCESSFUL))
                {
                    Ok(new { message = "Cập nhật trạng thái đã được cập nhật trước đó." });
                }

                if (order.StatusPayment.Equals(Payment_Status.PENDING))
                {
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

                        // NOTI
                        NotificationDto notificationCusDto = new NotificationDto()
                        {
                            NotificationId = 0,
                            Uid = orderPaidEmail.CustomerId,
                            CreatedAt = DateTime.Now,
                            Content = Notification_Content.ORDER_PAYMENT_THIRTPART(orderPaidEmail.TotalAmount),
                            Type = Notification_Type.ORDER,
                            IsRead = false,
                        };
                        NotificationDto notificationResDto = new NotificationDto()
                        {
                            NotificationId = 0,
                            Uid = orderPaidEmail.RestaurantId,
                            CreatedAt = DateTime.Now,
                            Content = Notification_Content.ORDER_PAYMENT_THIRTPART(orderPaidEmail.TotalAmount),
                            Type = Notification_Type.ORDER,
                            IsRead = false,
                        };

                        var notificationCus = await _notificationService.AddAsync(notificationCusDto);
                        var notificationRes = await _notificationService.AddAsync(notificationResDto);


                        if (notificationCus != null && notificationRes != null)
                        {
                            List<NotificationDto> notifications = new List<NotificationDto> { notificationCus, notificationRes };
                            await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                        }

                        await _sendMailService.SendEmailAsync(orderPaidEmail.Email, Email_Subject.ORDERCONFIRM, EmailTemplates.Order(orderPaidEmail));

                        return result
                                ? Ok(new { message = "Cập nhật trạng thái giao dịch thành công." })
                                : BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
                    }
                }
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
                var orderMenus = await _orderMenuService.GetItemsOriginalByOrderAsync(orderDto.OrderId);
                var orderMenusAdd = await _orderMenuService.GetItemsAddByOrderAsync(orderDto.OrderId);

                orderDto.OrderTables = orderTables;
                orderDto.OrderMenus = orderMenus;
                orderDto.OrderMenusAdd = orderMenusAdd;

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
                item.OrderMenus = await _orderMenuService.GetItemsOriginalByOrderAsync(item.OrderId);
                item.OrderTables = await _orderTableService.GetItemsByOrderAsync(item.OrderId);
                item.OrderMenusAdd = await _orderMenuService.GetItemsAddByOrderAsync(item.OrderId);
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

            var restaurant = await _restaurantRepository.QueryableAsync();

            foreach (var item in result)
            {
                var restaurantEntity = await restaurant.FirstOrDefaultAsync(x => x.Uid == item.RestaurantId);
                item.NameRes = restaurantEntity?.NameRes;
                item.OrderMenus = await _orderMenuService.GetItemsOriginalByOrderAsync(item.OrderId);
                item.OrderTables = await _orderTableService.GetItemsByOrderAsync(item.OrderId);
                item.OrderMenusAdd = await _orderMenuService.GetItemsAddByOrderAsync(item.OrderId);
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
            if (result != null)
            {
                var notiStatus = string.Empty;

                if (result.TotalAmount > 0 && status.Equals(Order_Status.CONFIRM))
                {
                    notiStatus = "đã được chấp nhận từ nhà hàng, hãy thanh toán thôi nào!";
                }
                
                if(result.TotalAmount <= 0 && status.Equals(Order_Status.CONFIRM))
                {
                    notiStatus = "đã được chấp nhận từ nhà hàng.";
                }

                if (status.Equals(Order_Status.COMPLETE))
                {
                    notiStatus = "đã hoàn thành";

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
                                TransactionAmount = completeOrder.TotalPaymentAmount ?? 0,
                                TransactionType = Transaction_Type.SYSTEMADD,
                                TransactionDate = DateTime.Now,
                                Description = Payment_Descriptions.SystemAddtractDescription(completeOrder.TotalPaymentAmount ?? 0),
                                Status = Payment_Status.SUCCESSFUL,
                            };

                            await _walletTransactionService.AddAsync(walletTransactionDto);
                        }
                    }
                }


                NotificationDto notificationDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = result.CustomerId ?? 0,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.ORDER_UPDATESTATUS(result.TotalAmount ?? 0, notiStatus),
                    Type = Notification_Type.ORDER,
                    IsRead = false,
                };

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
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

            // Xử lý tài khoản ví dựa trên vai trò người dùng
            var isCustomer = cancelOrder.RoleName.Equals(User_Role.CUSTOMER);

            if (cancelOrder.TotalAmount <= 0)
            {
                NotificationDto notificationDtoFree = new NotificationDto()
                {
                    NotificationId = 0,
                    CreatedAt = DateTime.Now,
                    Type = Notification_Type.ORDER,
                    IsRead = false,
                };

                if (isCustomer == true)
                {
                    notificationDtoFree.Uid = cancelOrder.RestaurantID;
                    notificationDtoFree.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "khách hàng");

                    var notification = await _notificationService.AddAsync(notificationDtoFree);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }
                }
                else
                {
                    notificationDtoFree.Uid = cancelOrder.CustomerID;
                    notificationDtoFree.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "nhà hàng");

                    var notification = await _notificationService.AddAsync(notificationDtoFree);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }
                }

                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.");
            }

            var checkStatusOrder = await _orderService.GetItemAsync(cancelOrderRequest.OrderId,cancelOrderRequest.UserId);

            if (checkStatusOrder.PaidAt == null && string.IsNullOrEmpty(checkStatusOrder.PaidAt.ToString()))
            {
                NotificationDto notificationDtoFree = new NotificationDto()
                {
                    NotificationId = 0,
                    CreatedAt = DateTime.Now,
                    Type = Notification_Type.ORDER,
                    IsRead = false,
                };

                if (isCustomer == true)
                {
                    notificationDtoFree.Uid = cancelOrder.RestaurantID;
                    notificationDtoFree.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "khách hàng");

                    var notification = await _notificationService.AddAsync(notificationDtoFree);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }
                }
                else
                {
                    notificationDtoFree.Uid = cancelOrder.CustomerID;
                    notificationDtoFree.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "nhà hàng");

                    var notification = await _notificationService.AddAsync(notificationDtoFree);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }
                }

                return Ok($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.");
            }


            if(isCustomer == false)
            {
                await _restaurantService.UpdateReputationScore(cancelOrder.RestaurantID);
            };

            var transactionAmount = cancelOrder.TotalPaymentAmount;

            if (isCustomer == true && cancelOrder.CancellationFeePercent > 0)
            {
                transactionAmount = cancelOrder.TotalPaymentAmount * (1 - (cancelOrder.CancellationFeePercent / 100));
            }

            var amountDifference = cancelOrder.TotalPaymentAmount - transactionAmount;

            // Kiểm tra nếu khách hàng hủy thì sẽ cộng tiền cho nhà hàng
            if ((isCustomer == true) && amountDifference > 0)
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
                    NotificationDto notificationSYSTEMADD = new NotificationDto()
                    {
                        NotificationId = 0,
                        Uid = walletBalanceRestaurantDto.Uid,
                        CreatedAt = DateTime.Now,
                        Content = Notification_Content.SYSTEM_ADD(amountDifference??0),
                        Type = Notification_Type.SYSTEM_ADD,
                        IsRead = false,
                    };

                    var notification = await _notificationService.AddAsync(notificationSYSTEMADD);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }

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
                NotificationDto notificationSYSTEMADD = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = cancelOrder.CustomerID,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.SYSTEM_ADD(transactionAmount ?? 0),
                    Type = Notification_Type.SYSTEM_ADD,
                    IsRead = false,
                };

                var notification = await _notificationService.AddAsync(notificationSYSTEMADD);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

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
            var recipientEmail = (isCustomer == true) ? cancelOrder.EmailRestaurant : cancelOrder.EmailCustomer;

            OrderPaidEmail orderPaidEmail = await _orderService.GetOrderPaid(cancelOrderRequest.OrderId);

            NotificationDto notificationDto = new NotificationDto()
            {
                NotificationId = 0,
                CreatedAt = DateTime.Now,
                Type = Notification_Type.ORDER,
                IsRead = false,
            };


            if (isCustomer == true)
            {
                notificationDto.Uid = cancelOrder.RestaurantID;
                notificationDto.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "khách hàng");

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }
                await _sendMailService.SendEmailAsync(recipientEmail, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrder(orderPaidEmail, Order_Status.CANCEL));
            }
            else
            {
                notificationDto.Uid = cancelOrder.CustomerID;
                notificationDto.Content = Notification_Content.ORDER_CANCEL(cancelOrder.TotalAmount ?? 0, "nhà hàng");

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }
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
                        OrderMenuId = 0,
                        OrderId = changeOrderMenu.OrderId,
                        MenuId = orderMenu.MenuId,
                        Quantity = orderMenu.Quantity,
                        Price = menu.Price,
                        OrderMenuType = OrderMenu_Type.ORIGINAL
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

            var order = await _orderService.GetItemAsync(changeOrderMenu.OrderId, changeOrderMenu.CustomerId);

            if (order != null)
            {
                if (order.DiscountId != null && order.DiscountId.HasValue)
                {
                    decimal foodAmount = 0;

                    OrderModel orderModel = new OrderModel()
                    {
                        CustomerId = order.CustomerId ?? 0,
                        DiscountId = order.DiscountId,
                        RestaurantId = order.RestaurantId ?? 0,
                        NameReceiver = order.NameReceiver,
                        ContentReservation = order.ContentReservation,
                        DateReservation = order.DateReservation,
                        NumberChild = order.NumberChild,
                        PhoneReceiver = order.PhoneReceiver,
                        TimeReservation = order.TimeReservation,
                        OrderMenus = changeOrderMenu.orderMenus
                    };

                    foodAmount += await CalculateTotalAmountForDiscountAsync(orderModel);

                    var resultUpdateOrder = await _orderService.UpdateFoodAmountChangeMenuAsync(changeOrderMenu.OrderId, foodAmount);

                    if (resultUpdateOrder)
                    {
                        return Ok("Successfully add menus and updated the order.");
                    }
                }
                else
                {
                    decimal foodAmount = 0;

                    CaculatorOrderDto caculatorOrder = new CaculatorOrderDto()
                    {
                        CustomerId = changeOrderMenu.CustomerId,
                        RestaurantId = changeOrderMenu.RestaurantId,
                        OrderMenus = changeOrderMenu.orderMenus
                    };

                    foodAmount += await CalculateMenuTotalForPreOrderAsync(caculatorOrder);

                    var resultUpdateOrder = await _orderService.UpdateFoodAmountChangeMenuAsync(changeOrderMenu.OrderId, foodAmount);

                    if (resultUpdateOrder)
                    {
                        return Ok("Successfully add menus and updated the order.");
                    }
                }
            }

            return BadRequest("Failed to update the order!");
        }


        [HttpPut("AddMenus")]
        public async Task<IActionResult> AddMenusAsync(ChangeOrderMenuDto changeOrderMenu)
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
                        OrderMenuId = 0, 
                        OrderId = changeOrderMenu.OrderId,
                        MenuId = orderMenu.MenuId,
                        Quantity = orderMenu.Quantity,
                        Price = menu.Price,
                        OrderMenuType = OrderMenu_Type.ADD
                    });
                }
            }

            if (createOrUpdateOrderMenuDtos.Count == 0)
            {
                return BadRequest("No valid menus found for the order.");
            }

            var result = await _orderMenuService.AddMenusAsync(createOrUpdateOrderMenuDtos);

            if (!result)
            {
                return BadRequest("Failed to change menus.");
            }

            var order = await _orderService.GetItemAsync(changeOrderMenu.OrderId, changeOrderMenu.CustomerId);

            if (order != null)
            {
                if(order.DiscountId != null && order.DiscountId.HasValue)
                {
                    decimal addFoodAmount = 0;

                    OrderModel orderModel = new OrderModel()
                    {
                        CustomerId = order.CustomerId ?? 0,
                        DiscountId = order.DiscountId,
                        RestaurantId = order.RestaurantId ?? 0,
                        NameReceiver = order.NameReceiver,
                        ContentReservation = order.ContentReservation,
                        DateReservation = order.DateReservation,
                        NumberChild = order.NumberChild,
                        PhoneReceiver = order.PhoneReceiver,
                        TimeReservation = order.TimeReservation,
                        OrderMenus = changeOrderMenu.orderMenus
                    };

                    addFoodAmount += await CalculateTotalAmountForDiscountAsync(orderModel);

                    var resultUpdateOrder = await _orderService.UpdateAddFoodAmountChangeMenuAsync(changeOrderMenu.OrderId, addFoodAmount);

                    if (resultUpdateOrder)
                    {
                        return Ok("Successfully add menus and updated the order.");
                    }
                }
                else
                {
                    decimal addFoodAmount = 0;

                    CaculatorOrderDto caculatorOrder = new CaculatorOrderDto()
                    {
                        CustomerId = changeOrderMenu.CustomerId,
                        RestaurantId = changeOrderMenu.RestaurantId,
                        OrderMenus = changeOrderMenu.orderMenus
                    };

                    addFoodAmount += await CalculateMenuTotalForPreOrderAsync(caculatorOrder);

                    var resultUpdateOrder = await _orderService.UpdateAddFoodAmountChangeMenuAsync(changeOrderMenu.OrderId, addFoodAmount);

                    if (resultUpdateOrder)
                    {
                        return Ok("Successfully add menus and updated the order.");
                    }
                }
            }

            return BadRequest("Failed to update the order!");
        }

        [HttpPut("ReturnFeePercentApplyCustomerFee")]
        public async Task<decimal> ApplyCustomerFeeAsync(int customerId, int restaurantId)
        {
            var restaurantPolicy = await _restaurantPolicyService.GetActivePolicyAsync(restaurantId);

            if (restaurantPolicy != null)
            {
                if (restaurantPolicy.FirstFeePercent != 0 || restaurantPolicy.ReturningFeePercent != 0)
                {
                    var isFirst = await _orderService.CheckIsFirstOrderAsync(customerId, restaurantId);
                    if (isFirst && restaurantPolicy.FirstFeePercent.HasValue && restaurantPolicy.FirstFeePercent.Value > 0)
                    {
                        return restaurantPolicy.FirstFeePercent.Value;
                    }
                    else if (!isFirst && restaurantPolicy.ReturningFeePercent.HasValue && restaurantPolicy.ReturningFeePercent.Value > 0)
                    {
                        return restaurantPolicy.ReturningFeePercent.Value;
                    }
                }
            }
            return 0;
        }


        [HttpPut("HandleReportOrder/{orderID}")]
        [SwaggerOperation(Summary = "Xử lý report Order: Admin")]
        public async Task<IActionResult> HandleReportOrder(int orderID)
        {
            var orderCheck = await _orderRepository.GetByIdAsync(orderID);
            if(orderCheck != null && orderCheck.StatusOrder == Order_Status.PAID)
            {
                var result = await _orderService.UpdateStatusAsync(orderID, Order_Status.CANCEL);
                if (result != null)
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
                                TransactionAmount = completeOrder.TotalPaymentAmount ?? 0,
                                TransactionType = Transaction_Type.SYSTEMADD,
                                TransactionDate = DateTime.Now,
                                Description = Payment_Descriptions.SystemAddtractDescription(completeOrder.TotalPaymentAmount ?? 0),
                                Status = Payment_Status.SUCCESSFUL,
                            };

                            await _walletTransactionService.AddAsync(walletTransactionDto);
                        }

                        NotificationDto notificationCusDto = new NotificationDto()
                        {
                            NotificationId = 0,
                            Uid = result.CustomerId ?? 0,
                            CreatedAt = DateTime.Now,
                            Content = Notification_Content.ORDER_REPORT_CUSTOMER(result.TotalAmount ?? 0),
                            Type = Notification_Type.ORDER,
                            IsRead = false,
                        };

                        NotificationDto notificationResDto = new NotificationDto()
                        {
                            NotificationId = 0,
                            Uid = result.RestaurantId ?? 0,
                            CreatedAt = DateTime.Now,
                            Content = Notification_Content.ORDER_REPORT_RESTAURANT(result.TotalAmount ?? 0),
                            Type = Notification_Type.ORDER,
                            IsRead = false,
                        };

                        var notificationRes = await _notificationService.AddAsync(notificationResDto);
                        var notificationCus = await _notificationService.AddAsync(notificationCusDto);

                        if (notificationRes != null && notificationCus != null)
                        {
                            List<NotificationDto> notifications = new List<NotificationDto> { notificationRes, notificationCus };
                            await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                        }

                        return Ok($"Cập nhật trạng thái cho đơn hàng với ID {orderID} thành công.");
                    }
                    return NotFound($"Đơn hàng với ID {orderID} không tồn tại.");
                }
                return BadRequest($"Không Update được đơn hàng.");
            }
            return BadRequest($"Đơn hàng đã bị thay đổi khi report! không thể xử lý.");
        }


    }
}
