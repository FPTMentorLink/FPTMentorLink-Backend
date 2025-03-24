using Services.Models.Request.Base;

namespace Services.Models.Request.MentorAvailability;

public class GetMentorAvailabilitiesRequest : PaginationQuery
{
    public Guid MentorId { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}