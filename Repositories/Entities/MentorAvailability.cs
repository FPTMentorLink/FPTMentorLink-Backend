using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class MentorAvailability : AuditableEntity
{
    [ForeignKey(nameof(Mentor))] public Guid MentorId { get; set; }
    public DateTime Date { get; set; } 
    public byte[] TimeMap { get; set; } = new byte[12]; // 96 slots (15 minutes each)
    public virtual Mentor Mentor { get; set; } = null!;

    // Check if a specific time slot is available
    public bool IsAvailable(int slotIndex)
    {
        if (slotIndex is < 0 or >= 96)
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Slot index must be between 0 and 95");

        var byteIndex = slotIndex / 8;
        var bitIndex = slotIndex % 8;
        return (TimeMap[byteIndex] & (1 << bitIndex)) != 0;
    }

    // Set availability for a specific time slot
    public void SetAvailability(int slotIndex, bool isAvailable)
    {
        if (slotIndex is < 0 or >= 96)
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Slot index must be between 0 and 95");

        var byteIndex = slotIndex / 8;
        var bitIndex = slotIndex % 8;
        
        if (isAvailable)
            TimeMap[byteIndex] |= (byte)(1 << bitIndex);
        else
            TimeMap[byteIndex] &= (byte)~(1 << bitIndex);
    }

    // Set availability for a time range
    public void SetAvailabilityRange(TimeSpan startTime, TimeSpan endTime, bool isAvailable)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);

        if (startSlot > endSlot)
            throw new ArgumentException("Start time must be before end time");

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            SetAvailability(slot, isAvailable);
        }
    }

    // Get all available time slots
    public IEnumerable<(TimeSpan Start, TimeSpan End)> GetAvailableTimeSlots()
    {
        var slots = new List<(TimeSpan Start, TimeSpan End)>();
        int? rangeStart = null;

        for (var slot = 0; slot < 96; slot++)
        {
            if (IsAvailable(slot))
            {
                if (rangeStart == null)
                    rangeStart = slot;
            }
            else
            {
                if (rangeStart.HasValue)
                {
                    slots.Add((GetTimeFromSlot(rangeStart.Value), GetTimeFromSlot(slot)));
                    rangeStart = null;
                }
            }
        }

        // Handle case where availability extends to end of day
        if (rangeStart.HasValue)
            slots.Add((GetTimeFromSlot(rangeStart.Value), new TimeSpan(24, 0, 0)));

        return slots;
    }

    // Convert time to slot index (15-minute intervals)
    public static int GetSlotFromTime(TimeSpan time)
    {
        return (int)(time.TotalMinutes / 15);
    }

    // Convert slot index to time
    public static TimeSpan GetTimeFromSlot(int slotIndex)
    {
        return TimeSpan.FromMinutes(slotIndex * 15);
    }

    // Clear all availability
    public void ClearAvailability()
    {
        Array.Clear(TimeMap, 0, TimeMap.Length);
    }

    // Set all slots as available
    public void SetAllAvailable()
    {
        for (var i = 0; i < TimeMap.Length; i++)
            TimeMap[i] = 0xFF;
    }

    // Check if any slots are available in a time range
    public bool HasAvailabilityInRange(TimeSpan startTime, TimeSpan endTime)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            if (IsAvailable(slot))
                return true;
        }

        return false;
    }

    // Count available slots in a time range
    public int CountAvailableSlots(TimeSpan startTime, TimeSpan endTime)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);
        var count = 0;

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            if (IsAvailable(slot))
                count++;
        }

        return count;
    }
}