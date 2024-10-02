using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // POST: api/Wallet/AddOTP
        [HttpPost("AddOTP")]
        public async Task<IActionResult> AddOTP([FromBody] WalletOtpDto walletOtpDto)
        {
            var result = await _walletService.AddOTPAsync(walletOtpDto);
            if (result)
                return Ok(new { message = "Thêm OTP thành công." });
            return BadRequest(new { message = "Thêm OTP thất bại." });
        }

        // GET: api/Wallet/CheckExistOTP/{Uid}
        [HttpGet("CheckExistOTP/{uid}")]
        public async Task<IActionResult> CheckExistOTP(int uid)
        {
            var exists = await _walletService.CheckExistOTP(uid);
            if (exists)
                return Ok(new { message = "OTP đã tồn tại." });
            return NotFound(new { message = "OTP không tồn tại." });
        }

        // POST: api/Wallet/CheckOTP/{uid}
        [HttpPost("CheckOTP/{uid}")]
        public async Task<IActionResult> CheckOTP(int uid, [FromQuery] string Otp)
        {
            var isValid = await _walletService.CheckOTP(uid, Otp);
            if (isValid)
                return Ok(new { message = "OTP hợp lệ." });
            return BadRequest(new { message = "OTP không hợp lệ." });
        }

        [HttpPut("UpdateBank")]
        public async Task<IActionResult> UpdateWalletBank([FromBody] WalletBankDto walletBankDto)
        {
            var result = await _walletService.UpdateWalletBankAsync(walletBankDto);
            if (result)
                return Ok(new { message = "Cập nhật thông tin ngân hàng thành công." });
            return BadRequest(new { message = "Cập nhật thông tin ngân hàng thất bại." });
        }

    }
}
