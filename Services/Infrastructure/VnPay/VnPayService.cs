using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.Models.VnPay;
using Services.Settings;

namespace Services.Infrastructure.VnPay;

public class VnPayService : IVnPayService
{
    private readonly VnPayLibrary _vnpay;
    private readonly VnPaySettings _vnPaySettings;
    private readonly IServiceProvider _serviceProvider;

    public VnPayService(VnPayLibrary vnpay, IOptions<VnPaySettings> vnPaySettings, IServiceProvider serviceProvider)
    {
        _vnpay = vnpay;
        _vnPaySettings = vnPaySettings.Value;
        _serviceProvider = serviceProvider;
        _vnpay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
        _vnpay.AddRequestData("vnp_Version", _vnPaySettings.Version);
        _vnpay.AddRequestData("vnp_Command", _vnPaySettings.Command);
        _vnpay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
        _vnpay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
        _vnpay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
    }

    public string? CreatePaymentLink(CreateVnPayPaymentModel model)
    {
        try
        {
            // Clear all request data before adding new data
            _vnpay.ClearRequestSpecificData();
            _vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            var vnTimeZoneOffset = new TimeSpan(7, 0, 0); // UTC+7 offset
            var createDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero).ToOffset(vnTimeZoneOffset);
            var expireDate = createDate.AddMinutes(15);

            _vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_OrderInfo", model.OrderInfo);
            _vnpay.AddRequestData("vnp_OrderType", model.OrderType);
            _vnpay.AddRequestData("vnp_TxnRef", model.TxnRef.ToString());
            _vnpay.AddRequestData("vnp_IpAddr", model.IpAddr);
            var paymentUrl =
                _vnpay.CreateRequestUrl(_vnPaySettings.PaymentUrl, _vnPaySettings.HashSecret);
            return paymentUrl;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public VnPayIpnResponse VerifyIpnPayment(IQueryCollection request)
    {
        if (request.Count == 0)
        {
            return new VnPayIpnResponse
            {
                RspCode = "99", // invalid request
                Message = "Input data required"
            };
        }
        _vnpay.ClearResponseData();
        foreach (var vnp in request.Where(x
                     => x.Key.StartsWith("vnp_") && !string.IsNullOrEmpty(x.Value)))
        {
            _vnpay.AddResponseData(vnp.Key, vnp.Value + "");
        }

        var vnpSecureHash = request["vnp_SecureHash"];
        var isValidSecureHash = _vnpay.ValidateSignature(vnpSecureHash!, _vnPaySettings.HashSecret);
        if (!isValidSecureHash)
        {
            return new VnPayIpnResponse
            {
                RspCode = "97", // invalid signature
                Message = "Invalid signature"
            };
        }

        // handle IPN response

        // var orderCode = long.Parse(_vnpay.GetResponseData("vnp_TxnRef"));
        // var order = await orderRepository.FindAll(x => x.TransactionCode == orderCode)
        //     .Include(x => x.OrderDetails)
        //     .ThenInclude(x => x.Product).FirstOrDefaultAsync();
        // if (order == null)
        // {
        //     return new VnPayIpnResponse
        //     {
        //         RspCode = "01", // order not found
        //         Message = "Order not found"
        //     };
        // }
        //
        // if (order.Status != Domain.Enums.OrderStatus.Pending)
        // {
        //     return new VnPayIpnResponse
        //     {
        //         RspCode = "02", // order already confirmed
        //         Message = "Order already confirmed"
        //     };
        // }
        //
        // var amount = int.Parse(_vnpay.GetResponseData("vnp_Amount")) / 100;
        // if (order.TotalAmount != amount)
        // {
        //     return new VnPayIpnResponse
        //     {
        //         RspCode = "04", // invalid amount
        //         Message = "invalid amount"
        //     };
        // }
        //
        // var responseCode = _vnpay.GetResponseData("vnp_ResponseCode");
        // var transactionStatus = _vnpay.GetResponseData("vnp_TransactionStatus");
        // if (responseCode == "00" && transactionStatus == "00")
        // {
        //     order.Status = Domain.Enums.OrderStatus.Processing;
        //     order.OrderLogs.Add(new OrderLog
        //     {
        //         // OrderId = order.Id,
        //         Status = Domain.Enums.OrderStatus.Processing,
        //         CreatedDate = DateTime.UtcNow
        //     });
        // }
        // else
        // {
        //     order.Status = Domain.Enums.OrderStatus.Cancelled;
        //     order.OrderLogs.Add(new OrderLog
        //     {
        //         // OrderId = order.Id,
        //         Status = Domain.Enums.OrderStatus.Cancelled,
        //         CreatedDate = DateTime.UtcNow
        //     });
        //     foreach (var product in order.OrderDetails)
        //     {
        //         product.Product.Quantity += product.Quantity;
        //         if (product.Product.Status == Domain.Enums.ProductStatus.OutOfStock)
        //         {
        //             product.Product.Status = Domain.Enums.ProductStatus.Selling;
        //         }
        //
        //         productRepository.Update(product.Product);
        //     }
        // }
        //
        // orderRepository.Update(order);
        // await unitOfWork.SaveChangesAsync(true, true);
        //
        // // Add SignalR notification
        // var connectionId = await _presenceTracker.GetConnectionsForUser(order.CustomerId);
        // if (connectionId != null)
        // {
        //     await _presenceHub.Clients.Clients(connectionId).SendAsync("OrderUpdated", order.Status);
        // }
        //
        // return new VnPayIpnResponse
        // {
        //     RspCode = "00", // success
        //     Message = "Confirm Success"
        // };

        return new VnPayIpnResponse
        {
            RspCode = "00",
            Message = "Confirm Success"
        };
    }
}