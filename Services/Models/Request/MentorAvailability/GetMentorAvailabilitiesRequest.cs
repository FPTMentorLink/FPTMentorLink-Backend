using Services.Models.Request.Base;

namespace Services.Models.Request.MentorAvailability;

public class GetMentorAvailabilitiesRequest : PaginationQuery
{
    public Guid MentorId { get; set; }
    public DateTime? Date { get; set; }
}