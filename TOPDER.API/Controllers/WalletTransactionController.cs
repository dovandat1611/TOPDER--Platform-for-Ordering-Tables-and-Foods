using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Net.payOS.Types;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Notification;
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
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<AppHub> _signalRHub;
        private readonly INotificationService _notificationService;


        public WalletTransactionController(IWalletTransactionService walletTransactionService, 
            IWalletService walletService, IPaymentGatewayService paymentGatewayService, IConfiguration configuration
            , IHubContext<AppHub> signalRHub, INotificationService notificationService)
        {
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _paymentGatewayService = paymentGatewayService;
            _configuration = configuration;
            _signalRHub = signalRHub;
            _notificationService = notificationService;
        }

        [HttpPost("Withdraw")]
        [SwaggerOperation(Summary = "Rút tiền ra khỏi ví: Customer")]
        public async Task<IActionResult> Withdraw([FromBody] WalletTransactionWithDrawDto walletTransactionWithDraw)
        {

            var balance = await _walletService.GetBalanceAsync(walletTransactionWithDraw.WalletId, walletTransactionWithDraw.Uid);

            if (balance <= 0)
            {
                return BadRequest(new { message = "Tạo giao dịch thất bại: Số dư trong ví không hợp lệ." });
            }

            if (balance < walletTransactionWithDraw.TransactionAmount)
            {
                return BadRequest(new { message = "Số dư trong ví không đủ để thực hiện giao dịch." });
            }

            WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
            {
                Uid = walletTransactionWithDraw.Uid,
                WalletId = walletTransactionWithDraw.WalletId,
                WalletBalance = balance - walletTransactionWithDraw.TransactionAmount
            };

            var updateWallet = await _walletService.UpdateWalletBalanceAsync(walletBalanceDto);

            if (updateWallet)
            {
                WalletTransactionDto walletTransactionDto = new WalletTransactionDto()
                {
                    TransactionId = 0,
                    Uid = walletTransactionWithDraw.Uid,
                    WalletId = walletTransactionWithDraw.WalletId,
                    TransactionAmount = walletTransactionWithDraw.TransactionAmount,
                    TransactionDate = DateTime.Now,
                    TransactionType = Transaction_Type.WITHDRAW,
                    Description = Payment_Descriptions.WithdrawalDescription(walletTransactionWithDraw.TransactionAmount),
                    Status = Payment_Status.PENDING
                };

                var result = await _walletTransactionService.AddAsync(walletTransactionDto);

                if (result)
                {
                    return Ok(new { message = "Tạo giao dịch và cập nhật số dư thành công." });
                }
                return BadRequest(new { message = "Tạo giao dịch thất bại sau khi cập nhật số dư." });
            }

            return BadRequest(new { message = "Cập nhật số dư thất bại." });
        }

        [HttpPut("CheckRecharge")]
        [SwaggerOperation(Summary = "Khi đã chuyển khoản xong thì sẽ trả về trạng thái thanh toán, sau đó sẽ check xem là thanh toán (Cancelled or Successful)")]
        public async Task<IActionResult> CheckRecharge(int transactionId, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            if (status.Equals(Payment_Status.CANCELLED))
            {
                var result = await _walletTransactionService.UpdateStatus(transactionId, status);
                return result
                    ? Ok(new { message = "Cập nhật trạng thái giao dịch thành công." })
                    : BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
            }

            if (status.Equals(Payment_Status.SUCCESSFUL))
            {
                var getWalletBalance = await _walletTransactionService.GetWalletBalanceAsync(transactionId);

                var updateWallet = await _walletService.UpdateWalletBalanceAsync(getWalletBalance);

                if (updateWallet)
                {
                    // NOTI
                    NotificationDto notificationDto = new NotificationDto()
                    {
                        NotificationId = 0,
                        Uid = getWalletBalance.Uid,
                        CreatedAt = DateTime.Now,
                        Content = Notification_Content.RECHARGE(getWalletBalance.TransactionAmount ?? 0),
                        Type = Notification_Type.RECHARGE,
                        IsRead = false,
                    };

                    var notification = await _notificationService.AddAsync(notificationDto);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification};
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }

                    var result = await _walletTransactionService.UpdateStatus(transactionId, status);
                    return result
                        ? Ok(new { message = "Cập nhật trạng thái giao dịch thành công." })
                        : BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
                }

                return BadRequest(new { message = "Cập nhật số dư ví thất bại." });
            }

            return BadRequest(new { message = "Trạng thái không hợp lệ. Vui lòng chọn CANCELLED hoặc SUCCESSFUL." });
        }


        [HttpPost("Recharge")]
        [SwaggerOperation(Summary = "Nạp tiền:(Số tiền phải lớn hơn 0 và không được dưới 10.000VNĐ.) cổng thanh toán phải là VIETQR hoặc VNPAY")]
        public async Task<IActionResult> Recharge([FromBody] RechargeWalletTransaction rechargeWalletTransaction)
        {
            // Kiểm tra số tiền
            if (rechargeWalletTransaction.TransactionAmount <= 0 || rechargeWalletTransaction.TransactionAmount < 10000)
            {
                return BadRequest(new { message = "Số tiền phải lớn hơn 0 và không được dưới 10.000VNĐ." });
            }

            // Kiểm tra cổng thanh toán
            if (!rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VIETQR) &&
                !rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VNPAY))
            {
                return BadRequest(new { message = "Cổng thanh toán không hợp lệ. Vui lòng chọn VietQR hoặc VNPay." });
            }


            var walletTransactionDto = new WalletTransactionDto()
            {
                TransactionId = 0,
                Uid = rechargeWalletTransaction.Uid,
                WalletId = rechargeWalletTransaction.WalletId,
                TransactionAmount = rechargeWalletTransaction.TransactionAmount,
                TransactionType = Transaction_Type.RECHARGE,
                TransactionDate = DateTime.UtcNow,
                Description = Payment_Descriptions.RechargeDescription(rechargeWalletTransaction.TransactionAmount),
                Status = Payment_Status.PENDING
            };

            var result = await _walletTransactionService.AddRechargeAsync(walletTransactionDto);

            if (result != null)
            {
                string linkPayment = string.Empty;
                int walletTransactionId = result.TransactionId;
                // Xử lý cho VietQR
                if (rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VIETQR))
                {
                    var items = new List<ItemData>
                    {
                        new ItemData(Transaction_Type.RECHARGE, (int)rechargeWalletTransaction.TransactionAmount, 1)
                    };

                    var paymentData = new PaymentData(
                        orderCode: GenerateOrderCodeForVIETQR.GenerateOrderCode(result.TransactionId, 36112002),
                        amount: (int)rechargeWalletTransaction.TransactionAmount,
                        description: Payment_Descriptions.RechargeVIETQRDescription(),
                        items: items,
                        cancelUrl: _configuration["PayOSSettings:CancelUrl"]+$"&transactionId={walletTransactionId}&paymentType=transaction",
                        returnUrl: _configuration["PayOSSettings:ReturnUrl"]+$"&transactionId={walletTransactionId}&paymentType=transaction"
                    );

                    CreatePaymentResult createPayment = await _paymentGatewayService.CreatePaymentUrlPayOS(paymentData);
                    linkPayment = createPayment.checkoutUrl;
                }
                // Xử lý cho VNPay
                else if (rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VNPAY))
                {
                    var paymentInformationModel = new PaymentInformationModel()
                    {
                        BookingID = result.TransactionId.ToString(),
                        AccountID = rechargeWalletTransaction.Uid.ToString(),
                        CustomerName = rechargeWalletTransaction.Uid.ToString(),
                        Amount = (double)rechargeWalletTransaction.TransactionAmount,
                        PaymentType = "Transaction"
                    };

                    linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext);
                }

                if (!string.IsNullOrEmpty(linkPayment))
                {
                    return Ok(new
                    {
                        linkPayment,
                        walletTransactionId
                    });
                }
            }

            return BadRequest(new { message = "Tạo giao dịch thất bại." });
        }


        [HttpGet("GetWalletTransactionList/{uid}")]
        [SwaggerOperation(Summary = "Xem lịch sử giao dịch của riêng user: Customer | Restaurant")]
        public async Task<IActionResult> GetWalletTransactionList(int uid)
        {
            try
            {
                var result = await _walletTransactionService.GetWalletTransactionHistoryAsync(uid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", error = ex.Message });
            }
        }


        [HttpGet("GetWalletTransactionListForAdmin")]
        [SwaggerOperation(Summary = "Xem lịch sử giao dịch và đặc biệt là (Withdraw) phải làm thủ công bằng tay: Admin")]
        public async Task<IActionResult> GetPaging(string? status)
        {
            var result = await _walletTransactionService.GetWalletTransactionWithDrawAsync(status);
            return Ok(result);
        }

        [HttpPut("UpdateStatus/{transactionId}")]
        [SwaggerOperation(Summary = "Cập nhật lại trạng thái thanh toán rút tiền (Cancelled, Successful hoặc Pending): Admin")]
        public async Task<IActionResult> UpdateStatus(int transactionId, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            var validStatuses = new List<string>
            {
                Payment_Status.CANCELLED,
                Payment_Status.SUCCESSFUL
            };

            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ. Vui lòng chọn CANCELLED hoặc SUCCESSFUL." });
            }

            var result = await _walletTransactionService.UpdateStatus(transactionId, status);

            var getWalletBalance = await _walletTransactionService.GetWalletBalanceAsync(transactionId);

            if (result)
            {   
                if (status.Equals(Payment_Status.CANCELLED))
                {
                    var updateWallet = await _walletService.UpdateWalletBalanceAsync(getWalletBalance);

                    if (updateWallet)
                    {
                        NotificationDto notificationDto = new NotificationDto()
                        {
                            NotificationId = 0,
                            Uid = getWalletBalance.Uid,
                            CreatedAt = DateTime.Now,
                            Content = Notification_Content.WITHDRAW_FAIL(getWalletBalance.TransactionAmount ?? 0),
                            Type = Notification_Type.WITHDRAW,
                            IsRead = false,
                        };

                        var notification = await _notificationService.AddAsync(notificationDto);

                        if (notification != null)
                        {
                            List<NotificationDto> notifications = new List<NotificationDto> { notification };
                            await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                        }

                        return Ok(new { message = "Cập nhật trạng thái giao dịch thành công." });
                    }
                }

                if (status.Equals(Payment_Status.SUCCESSFUL))
                {

                    NotificationDto notificationDto = new NotificationDto()
                    {
                        NotificationId = 0,
                        Uid = getWalletBalance.Uid,
                        CreatedAt = DateTime.Now,
                        Content = Notification_Content.WITHDRAW_SUCCESSFUL(getWalletBalance.TransactionAmount ?? 0),
                        Type = Notification_Type.WITHDRAW,
                        IsRead = false,
                    };

                    var notification = await _notificationService.AddAsync(notificationDto);

                    if (notification != null)
                    {
                        List<NotificationDto> notifications = new List<NotificationDto> { notification };
                        await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                    }

                    return Ok(new { message = "Cập nhật trạng thái giao dịch thành công." });
                }
            }
            return BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
        }


    }
}
