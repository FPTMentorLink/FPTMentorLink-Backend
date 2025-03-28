using Services.Models.Request.Base;

namespace Services.Models.Request.Notification;

public class GetNotificationsRequest : PaginationQuery
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}