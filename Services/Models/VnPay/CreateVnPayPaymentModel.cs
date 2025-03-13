using System.ComponentModel.DataAnnotations;

namespace Services.Models.VnPay;

public class CreateVnPayPaymentModel
{
    public long TxnRef { get; set; }
    public int Amount { get; set; }
    [MaxLength(255)] public string OrderInfo { get; set; } = "";
    [MaxLength(100)] public string OrderType { get; set; } = "";
    public string IpAddr { get; set; }
}