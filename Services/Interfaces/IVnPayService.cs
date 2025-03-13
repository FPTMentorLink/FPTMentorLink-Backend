using Microsoft.AspNetCore.Http;
using Services.Models.VnPay;

namespace Services.Interfaces;

public interface IVnPayService
{
    Task<string?> CreatePaymentLink(CreateVnPayPaymentModel model);
    Task<VnPayIpnResponse> VerifyIpnPayment(IQueryCollection request);
}