using Microsoft.AspNetCore.Http;
using Services.Models.VnPay;

namespace Services.Interfaces;

public interface IVnPayService
{
    string? CreatePaymentLink(CreateVnPayPaymentModel model);
    VnPayIpnResponse VerifyIpnPayment(IQueryCollection request);
}