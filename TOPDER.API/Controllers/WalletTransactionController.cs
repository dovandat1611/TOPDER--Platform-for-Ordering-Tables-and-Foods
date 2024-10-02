using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
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
        private readonly IUserService _userService;


        public WalletTransactionController(IWalletTransactionService walletTransactionService, 
            IWalletService walletService, IPaymentGatewayService paymentGatewayService, IUserService userService)
        {
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _paymentGatewayService = paymentGatewayService;
            _userService = userService;
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WalletTransactionDto walletTransactionDto)
        {
            var balance = await _walletService.GetBalanceAsync(walletTransactionDto.WalletId, walletTransactionDto.Uid);

            if (balance <= 0)
            {
                return BadRequest(new { message = "Tạo giao dịch thất bại: Số dư trong ví không hợp lệ." });
            }

            if (balance < walletTransactionDto.TransactionAmount)
            {
                return BadRequest(new { message = "Số dư trong ví không đủ để thực hiện giao dịch." });
            }

            WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
            {
                Uid = walletTransactionDto.Uid,
                WalletId = walletTransactionDto.WalletId,
                WalletBalance = balance - walletTransactionDto.TransactionAmount
            };

            var updateWallet = await _walletService.UpdateWalletBalanceAsync(walletBalanceDto);

            if (updateWallet)
            {
                var result = await _walletTransactionService.AddAsync(walletTransactionDto);

                if (result)
                {
                    return Ok(new { message = "Tạo giao dịch và cập nhật số dư thành công." });
                }
                return BadRequest(new { message = "Tạo giao dịch thất bại sau khi cập nhật số dư." });
            }

            return BadRequest(new { message = "Cập nhật số dư thất bại." });
        }

        [HttpPut("CheckRecharge/{transactionId}")]
        public async Task<IActionResult> CheckRecharge(int transactionId, [FromBody] string status)
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
        public async Task<IActionResult> Recharge([FromBody] RechargeWalletTransaction rechargeWalletTransaction)
        {
            // Kiểm tra số tiền
            if (rechargeWalletTransaction.TransactionAmount <= 0 || rechargeWalletTransaction.TransactionAmount < 5000)
            {
                return BadRequest(new { message = "Số tiền phải lớn hơn 0 và không được dưới 5.000VNĐ." });
            }

            // Kiểm tra cổng thanh toán
            if (!rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VIETQR) &&
                !rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VNPAY))
            {
                return BadRequest(new { message = "Cổng thanh toán không hợp lệ. Vui lòng chọn VietQR hoặc VNPay." });
            }

            UserPayment userInformation;
            try
            {
                userInformation = await _userService.GetInformationUserToPayment(rechargeWalletTransaction.Uid);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            var walletTransactionDto = new WalletTransactionDto()
            {
                TransactionId = 0,
                Uid = rechargeWalletTransaction.Uid,
                WalletId = rechargeWalletTransaction.WalletId,
                TransactionAmount = rechargeWalletTransaction.TransactionAmount,
                TransactionType = Transaction_Type.RECHARGE,
                TransactionDate = DateTime.UtcNow,
                Description = Payment_Descriptions.RechargeDescription(userInformation.Name, userInformation.Id),
                Status = Payment_Status.PENDING
            };

            var result = await _walletTransactionService.AddRechargeAsync(walletTransactionDto);

            if (result != null)
            {
                string linkPayment = string.Empty;

                // Xử lý cho VietQR
                if (rechargeWalletTransaction.PaymentGateway.Equals(PaymentGateway.VIETQR))
                {
                    var items = new List<ItemData>
                    {
                        new ItemData(Transaction_Type.RECHARGE, (int)rechargeWalletTransaction.TransactionAmount, 1)
                    };

                    var paymentData = new PaymentData(
                        orderCode: result.TransactionId,
                        amount: (int)rechargeWalletTransaction.TransactionAmount,
                        description: Payment_Descriptions.RechargeDescription(userInformation.Name, userInformation.Id),
                        items: items,
                        cancelUrl: "https://yourapp.com/cancel",
                        returnUrl: "https://yourapp.com/return"
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
                        AccountID = userInformation.Id.ToString(),
                        CustomerName = userInformation.Name,
                        Amount = (double)rechargeWalletTransaction.TransactionAmount
                    };

                    linkPayment = await _paymentGatewayService.CreatePaymentUrlVnpay(paymentInformationModel, HttpContext);
                }

                if (!string.IsNullOrEmpty(linkPayment))
                {
                    return Ok(new { linkPayment });
                }
            }

            return BadRequest(new { message = "Tạo giao dịch thất bại." });
        }


        [HttpGet("RestaurantOrCustomer/list/{uid}")]
        public async Task<IActionResult> GetRestaurantOrCustomerPaging(int uid, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            try
            {
                var result = await _walletTransactionService.GetPagingAsync(pageNumber, pageSize, uid, status);
                var response = new PaginatedResponseDto<WalletTransactionDto>(result, result.PageIndex, result.TotalPages, result.HasPreviousPage, result.HasNextPage);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", error = ex.Message });
            }
        }


        [HttpGet("admin/list")]
        public async Task<IActionResult> GetPaging([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var result = await _walletTransactionService.GetAdminPagingAsync(pageNumber, pageSize, status);

            var response = new PaginatedResponseDto<WalletTransactionAdminDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpPut("UpdateStatus/{transactionId}")]
        public async Task<IActionResult> UpdateStatus(int transactionId, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            var validStatuses = new List<string>
            {
                Payment_Status.PENDING,
                Payment_Status.CANCELLED,
                Payment_Status.SUCCESSFUL
            };

            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ. Vui lòng chọn CANCELLED, SUCCESSFUL hoặc PENDING." });
            }

            var result = await _walletTransactionService.UpdateStatus(transactionId, status);

            if (result)
            {
                return Ok(new { message = "Cập nhật trạng thái giao dịch thành công." });
            }

            return BadRequest(new { message = "Cập nhật trạng thái giao dịch thất bại." });
        }


    }
}
