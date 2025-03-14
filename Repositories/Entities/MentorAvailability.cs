using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;
using Repositories.Utils;

namespace Repositories.Entities;

public class MentorAvailability : AuditableEntity
{
    [ForeignKey(nameof(Mentor))] public Guid MentorId { get; set; }
    public DateTime Date { get; set; } 
    public byte[] TimeMap { get; set; } = new byte[12]; // 96 slots (15 minutes each)
    public virtual Mentor Mentor { get; set; } = null!;

    // Wrapper methods that use TimeMapUtils
    public bool IsAvailable(int slotIndex) => TimeMap.IsAvailable(slotIndex);
    public void SetAvailability(int slotIndex, bool isAvailable) => TimeMap.SetAvailability(slotIndex, isAvailable);
    public void SetAvailabilityRange(TimeSpan startTime, TimeSpan endTime, bool isAvailable) => 
        TimeMap.SetAvailabilityRange(startTime, endTime, isAvailable);
    public IEnumerable<AvailableTimeSlot> GetAvailableTimeSlots() => TimeMap.GetAvailableTimeSlots();
    public void ClearAvailability() => TimeMap.ClearAvailability();
    public void SetAllAvailable() => TimeMap.SetAllAvailable();
    public bool HasAvailabilityInRange(TimeSpan startTime, TimeSpan endTime) => 
        TimeMap.HasAvailabilityInRange(startTime, endTime);
    public bool IsAvailabilityInRange(TimeSpan startTime, TimeSpan endTime) => 
        TimeMap.IsAvailableInRange(startTime, endTime);
    public int CountAvailableSlots(TimeSpan startTime, TimeSpan endTime) => 
        TimeMap.CountAvailableSlots(startTime, endTime);
}

public class AvailableTimeSlot
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public string FormattedStartTime => StartTime.Hours.ToString("D2") + ":" + StartTime.Minutes.ToString("D2");
    public string FormattedEndTime => EndTime.Hours.ToString("D2") + ":" + EndTime.Minutes.ToString("D2");
    public string DisplayText => $"{FormattedStartTime} - {FormattedEndTime}";
}