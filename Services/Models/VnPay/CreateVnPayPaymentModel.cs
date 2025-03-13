namespace Services.Models.VnPay;

public class CreateVnPayPaymentModel
{
    public long TxnRef { get; set; }
    public int Amount { get; set; }
    public string OrderInfo { get; set; }
    public string OrderType { get; set; }
    public string IpAddr { get; set; }
}