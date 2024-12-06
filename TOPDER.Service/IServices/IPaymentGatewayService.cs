using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.VNPAY;

namespace TOPDER.Service.IServices
{
    public interface IPaymentGatewayService
    {
        Task<string> CreatePaymentUrlVnpay(PaymentInformationModel request, HttpContext httpContext, string typePayment);
        Task<CreatePaymentResult> CreatePaymentUrlPayOS(PaymentData paymentData);
    }
}
