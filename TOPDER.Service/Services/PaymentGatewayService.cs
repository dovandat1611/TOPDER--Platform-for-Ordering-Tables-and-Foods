using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly PayOS _payOS;
        private readonly VNPayLibrary _vnPay;

        public PaymentGatewayService(IConfiguration configuration)
        {
            _configuration = configuration;

            _payOS = InitializePayOS();

            _vnPay = new VNPayLibrary();
        }

        private PayOS InitializePayOS()
        {
            var clientId = GetConfigValue("PayOSSettings:ClientId");
            var apiKey = GetConfigValue("PayOSSettings:ApiKey");
            var checksumKey = GetConfigValue("PayOSSettings:ChecksumKey");

            return new PayOS(clientId, apiKey, checksumKey);
        }

        private string GetConfigValue(string key) => _configuration[key];

        public async Task<CreatePaymentResult> CreatePaymentUrlPayOS(PaymentData paymentData)
        {
            try
            {
                return await _payOS.createPaymentLink(paymentData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating PayOS payment link", ex);
            }
        }

        public Task<string> CreatePaymentUrlVnpay(PaymentInformationModel requestDto, HttpContext httpContext, string typePayment)
        {
            try
            {
                var timeNow = GetCurrentTimeInTimeZone();


                var urlCallBack = $"{GetConfigValue("Vnpay:ReturnUrl")}/{requestDto.BookingID}";

                if (requestDto.PaymentType.Equals("Order"))
                {
                    urlCallBack += "?paymentType=order";
                }

                if (requestDto.PaymentType.Equals("Transaction"))
                {
                    urlCallBack += "?paymentType=transaction";
                }

                if (requestDto.PaymentType.Equals("Booking"))
                {
                    urlCallBack += "?paymentType=booking";
                }

                AddVnPayRequestData(requestDto, timeNow, urlCallBack, httpContext, typePayment);

                var paymentUrl = _vnPay.CreateRequestUrl(GetConfigValue("Vnpay:BaseUrl"), GetConfigValue("Vnpay:HashSecret"));

                return Task.FromResult(paymentUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating VNPay payment URL", ex);
            }
        }

        private DateTime GetCurrentTimeInTimeZone()
        {
            var timeZoneId = GetConfigValue("TimeZoneId");
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }

        private void AddVnPayRequestData(PaymentInformationModel requestDto, DateTime timeNow, string urlCallBack, HttpContext httpContext, string typePayment)
        {
            _vnPay.AddRequestData("vnp_Version", GetConfigValue("Vnpay:Version"));
            _vnPay.AddRequestData("vnp_Command", GetConfigValue("Vnpay:Command"));
            _vnPay.AddRequestData("vnp_TmnCode", GetConfigValue("Vnpay:TmnCode"));
            _vnPay.AddRequestData("vnp_Amount", ((int)requestDto.Amount * 100).ToString());  
            _vnPay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            _vnPay.AddRequestData("vnp_CurrCode", GetConfigValue("Vnpay:CurrCode"));
            _vnPay.AddRequestData("vnp_IpAddr", _vnPay.GetIpAddress(httpContext));
            _vnPay.AddRequestData("vnp_Locale", GetConfigValue("Vnpay:Locale"));
            _vnPay.AddRequestData("vnp_OrderInfo", $"Khach hang: {requestDto.CustomerName} thanh toan hoa don {requestDto.BookingID}");
            _vnPay.AddRequestData("vnp_OrderType", "other");
            _vnPay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            _vnPay.AddRequestData("vnp_TxnRef", requestDto.BookingID + "TOPDERTEST" + typePayment);
        }
    }
}
