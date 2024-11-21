using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.BookingAdvertisement;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Services;
using TOPDER.Service.Utils;
using Net.payOS.Types;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using Microsoft.AspNetCore.SignalR;
using TOPDER.Service.Hubs;
using TOPDER.Service.Dtos.Notification;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingAdvertisementController : ControllerBase
    {
        private readonly IBookingAdvertisementService _bookingAdvertisementService;
        private readonly IBookingAdvertisementRepository _bookingAdvertisementRepository;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _signalRHub;


        public BookingAdvertisementController(IBookingAdvertisementService bookingAdvertisementService,
            IBookingAdvertisementRepository bookingAdvertisementRepository,
            IWalletTransactionService walletTransactionService,
            IWalletService walletService,
            IUserService userService,
            IConfiguration configuration,
            IPaymentGatewayService paymentGatewayService, INotificationService notificationService, IHubContext<AppHub> signalRHub)
        {
            _bookingAdvertisementService = bookingAdvertisementService;
            _bookingAdvertisementRepository = bookingAdvertisementRepository;
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _userService = userService;
            _configuration = configuration;
            _paymentGatewayService = paymentGatewayService;
            _notificationService = notificationService;
            _signalRHub = signalRHub;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> AddBookingAdvertisement([FromBody] CreateBookingAdvertisementDto bookingAdvertisementDto)
        {
            if (bookingAdvertisementDto == null)
            {
                return BadRequest("Invalid data.");
            }

            bool isCreated = await _bookingAdvertisementService.AddAsync(bookingAdvertisementDto);
            if (isCreated)
            {
                //NotificationDto notificationDto = new NotificationDto()
                //{
                //    NotificationId = 0,
                //    Uid = 1,
                //    CreatedAt = DateTime.Now,
                //    Content = Notification_Content.BOOKING_CREATE(),
                //    Type = Notification_Type.BOOKING,
                //    IsRead = false,
                //};

                //var notification = await _notificationService.AddAsync(notificationDto);

                //if (notification != null)
                //{
                //    List<NotificationDto> notifications = new List<NotificationDto> { notificationDto };
                //    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                //}

                return Ok("Booking advertisement created successfully.");
            }
            return StatusCode(500, "An error occurred while creating the booking advertisement.");
        }

        [HttpGet]
        [Route("GetAvailableAdvertisementForRestaurant")]
        public async Task<ActionResult<List<BookingAdvertisementViewDto>>> GetAllBookingAdvertisementAvailable()
        {
            var bookingAdvertisements = await _bookingAdvertisementService.GetAllBookingAdvertisementAvailableAsync();
            return Ok(bookingAdvertisements);
        }


        [HttpGet]
        [Route("GetBookingAdvertisementForAdmin")]
        public async Task<ActionResult<List<BookingAdvertisementAdminDto>>> GetAllBookingAdvertisementForAdmin()
        {
            var bookingAdvertisements = await _bookingAdvertisementService.GetAllBookingAdvertisementForAdminAsync();
            return Ok(bookingAdvertisements);
        }

        [HttpPut("UpdateStatus/{bookingId}")]
        public async Task<IActionResult> UpdateStatus(int bookingId, [FromQuery] string status)
        {
            var isUpdated = await _bookingAdvertisementService.UpdateStatusAsync(bookingId, status);

            if (isUpdated != null)
            {
                NotificationDto notificationDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = isUpdated.RestaurantId,
                    CreatedAt = DateTime.Now,
                    Content = status == Booking_Status.CANCELLED ? Notification_Content.BOOKING_FAIL() : Notification_Content.BOOKING_SUCCRESSFUL(),
                    Type = Notification_Type.BOOKING,
                    IsRead = false,
                };

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

                return Ok(new { message = "Status updated successfully." });
            }

            return BadRequest(new { message = "Failed to update status. Please check the booking ID and status value." });
        }


        [HttpGet]
        [Route("GetBookingAdvertisementForRestaurant/{restaurantId}")]
        public async Task<ActionResult<List<BookingAdvertisementDto>>> GetAllBookingAdvertisementForRestaurant(int restaurantId)
        {
            var bookingAdvertisements = await _bookingAdvertisementService.GetAllBookingAdvertisementForRestaurantAsync(restaurantId);
            return Ok(bookingAdvertisements);
        }

        [HttpPut("CheckPayment/{bookingId}")]
        public async Task<IActionResult> UpdateStatusPayment(int bookingId, [FromQuery] string status)
        {
            var isUpdated = await _bookingAdvertisementService.UpdateStatusPaymentAsync(bookingId, status);

            if (isUpdated)
            {
                return Ok(new { message = "Status updated successfully." });
            }


            return BadRequest(new { message = "Failed to update status. Please check the booking ID and status value." });
        }


        [HttpPost("ChoosePaymentGatePaymentGateway")]
        [SwaggerOperation(Summary = "ISBALANCE | VIETQR | VNPAY")]
        public async Task<IActionResult> ChoosePaymentGatePaymentGateway(int bookingId, string paymentGateway)
        {
            BookingAdvertisement bookingAdvertisement;

            bookingAdvertisement = await _bookingAdvertisementRepository.GetByIdAsync(bookingId);

            if(bookingAdvertisement != null)
            {

                if (paymentGateway.Equals(PaymentGateway.ISBALANCE))
                {
                    return await HandleWalletPayment(bookingAdvertisement);
                }

                if (paymentGateway.Equals(PaymentGateway.VIETQR))
                {
                    return await HandleVietQRPayment(bookingAdvertisement);
                }

                if (paymentGateway.Equals(PaymentGateway.VNPAY))
                {
                    return await HandleVNPAYPayment(bookingAdvertisement);
                }
            }
            return BadRequest("Cổng thanh toán không hợp lệ hoặc không tìm thấy booking.");
        }

        private async Task<IActionResult> HandleWalletPayment(BookingAdvertisement bookingAdvertisement)
        {
            // Check wallet balance
            var walletBalance = await _walletService.GetBalanceOrderAsync(bookingAdvertisement.RestaurantId);

            if (walletBalance < bookingAdvertisement.TotalAmount)
            {
                return BadRequest("Số dư ví không đủ cho giao dịch này.");
            }

            decimal newWalletBalance = walletBalance - bookingAdvertisement.TotalAmount;

            UserOrderIsBalance userOrderIsBalance;
            try
            {
                userOrderIsBalance = await _userService.GetInformationUserOrderIsBalance(bookingAdvertisement.RestaurantId);
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
                TransactionAmount = bookingAdvertisement.TotalAmount,
                TransactionType = Transaction_Type.SYSTEMSUBTRACT,
                TransactionDate = DateTime.Now,
                Description = Payment_Descriptions.SystemSubtractDescription(bookingAdvertisement.TotalAmount),
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
                Uid = bookingAdvertisement.RestaurantId,
                WalletBalance = newWalletBalance
            };

            var updateWalletResult = await _walletService.UpdateWalletBalanceOrderAsync(walletBalanceOrder);

            if (!updateWalletResult)
            {
                return BadRequest("Cập nhật số dư ví thất bại.");
            }

            var isUpdated = await _bookingAdvertisementService.UpdateStatusPaymentAsync(bookingAdvertisement.BookingId, Payment_Status.SUCCESSFUL);

            if (!isUpdated)
            {
                 return BadRequest("Thay đổi trạng thái booking thất bại.");
            }

            return Ok("Thanh toán booking thành công");
        }

        private async Task<IActionResult> HandleVietQRPayment(BookingAdvertisement bookingAdvertisement)
        {
            var items = new List<ItemData>();

            items.Add(new ItemData("Đặt quảng cáo",1, (int)bookingAdvertisement.TotalAmount));

            var paymentData = new PaymentData(
                orderCode: GenerateOrderCodeForVIETQR.GenerateOrderCode(bookingAdvertisement.BookingId, 16112002),
                amount: (int)bookingAdvertisement.TotalAmount,
                description: Booking_PaymentContent.PaymentContentVIETQR(),
                items: items,
                cancelUrl: _configuration["PayOSSettings:CancelUrl"] + $"&transactionId={bookingAdvertisement.BookingId}&paymentType=booking",
                returnUrl: _configuration["PayOSSettings:CancelUrl"] + $"&transactionId={bookingAdvertisement.BookingId}&paymentType=booking"
            );

            CreatePaymentResult createPayment = await _paymentGatewayService.CreatePaymentUrlPayOS(paymentData);
            return Ok(createPayment.checkoutUrl);
        }

        private async Task<IActionResult> HandleVNPAYPayment(BookingAdvertisement bookingAdvertisement)
        {

            var paymentInformationModel = new PaymentInformationModel()
            {
                BookingID = bookingAdvertisement.BookingId.ToString(),
                AccountID = bookingAdvertisement.RestaurantId.ToString(),
                CustomerName = bookingAdvertisement.RestaurantId.ToString(),
                Amount = (int)bookingAdvertisement.TotalAmount,
                PaymentType = "Booking"
            };

            string linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext);
            return Ok(linkPayment);
        }

    }
}
