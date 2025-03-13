using Repositories.Entities;

namespace Repositories.Utils;

public static class TimeMapUtils
{
    public static bool IsTimeMapValid(this byte[] timeMap, int minAppointmentSessionLength)
    {
        var minConsecutiveBits = minAppointmentSessionLength / 15;
        if (minConsecutiveBits <= 1)
            return true;
        
        var consecutiveCount = 0;
        
        for (var byteIndex = 0; byteIndex < timeMap.Length; byteIndex++)
        {
            var currentByte = timeMap[byteIndex];
            
            if (currentByte == 0)
            {
                if (consecutiveCount > 0 && consecutiveCount < minConsecutiveBits)
                    return false;
                consecutiveCount = 0;
                continue;
            }
            
            if (currentByte == 0xFF)
            {
                consecutiveCount += 8;
                continue;
            }
            
            for (var bitIndex = 0; bitIndex < 8; bitIndex++)
            {
                var isAvailable = (currentByte & (1 << bitIndex)) != 0;
                
                if (isAvailable)
                    consecutiveCount++;
                else if (consecutiveCount > 0)
                {
                    if (consecutiveCount < minConsecutiveBits)
                        return false;
                    consecutiveCount = 0;
                }
            }
        }
        
        if (consecutiveCount > 0 && consecutiveCount < minConsecutiveBits)
            return false;
        
        return true;
    }

    public static bool IsAvailable(this byte[] timeMap, int slotIndex)
    {
        if (slotIndex is < 0 or >= 96)
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Slot index must be between 0 and 95");

        var byteIndex = slotIndex / 8;
        var bitIndex = slotIndex % 8;
        return (timeMap[byteIndex] & (1 << bitIndex)) != 0;
    }

    public static void SetAvailability(this byte[] timeMap, int slotIndex, bool isAvailable)
    {
        if (slotIndex is < 0 or >= 96)
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Slot index must be between 0 and 95");

        var byteIndex = slotIndex / 8;
        var bitIndex = slotIndex % 8;
        
        if (isAvailable)
            timeMap[byteIndex] |= (byte)(1 << bitIndex);
        else
            timeMap[byteIndex] &= (byte)~(1 << bitIndex);
    }

    public static void SetAvailabilityRange(this byte[] timeMap, TimeSpan startTime, TimeSpan endTime, bool isAvailable)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);

        if (startSlot > endSlot)
            throw new ArgumentException("Start time must be before end time");

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            SetAvailability(timeMap, slot, isAvailable);
        }
    }

    public static IEnumerable<AvailableTimeSlot> GetAvailableTimeSlots(this byte[] timeMap)
    {
        var slots = new List<AvailableTimeSlot>();
        int? rangeStart = null;

        for (var slot = 0; slot < 96; slot++)
        {
            if (IsAvailable(timeMap, slot))
            {
                rangeStart ??= slot;
            }
            else
            {
                if (rangeStart.HasValue)
                {
                    slots.Add(new AvailableTimeSlot
                    {
                        StartTime = GetTimeFromSlot(rangeStart.Value),
                        EndTime = GetTimeFromSlot(slot)
                    });
                    rangeStart = null;
                }
            }
        }

        if (rangeStart.HasValue)
        {
            slots.Add(new AvailableTimeSlot
            {
                StartTime = GetTimeFromSlot(rangeStart.Value),
                EndTime = new TimeSpan(24, 0, 0)
            });
        }

        return slots;
    }

    public static int GetSlotFromTime(TimeSpan time)
    {
        return (int)(time.TotalMinutes / 15);
    }

    public static TimeSpan GetTimeFromSlot(int slotIndex)
    {
        return TimeSpan.FromMinutes(slotIndex * 15);
    }

    public static DateTime GetDateTimeFromSlot(int slotIndex, DateTime date)
    {
        var time = GetTimeFromSlot(slotIndex);
        return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
    }

    public static void ClearAvailability(this byte[] timeMap)
    {
        Array.Clear(timeMap, 0, timeMap.Length);
    }

    public static void SetAllAvailable(this byte[] timeMap)
    {
        for (var i = 0; i < timeMap.Length; i++)
            timeMap[i] = 0xFF;
    }

    public static bool HasAvailabilityInRange(this byte[] timeMap, TimeSpan startTime, TimeSpan endTime)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            if (IsAvailable(timeMap, slot))
                return true;
        }

        return false;
    }

    public static int CountAvailableSlots(this byte[] timeMap, TimeSpan startTime, TimeSpan endTime)
    {
        var startSlot = GetSlotFromTime(startTime);
        var endSlot = GetSlotFromTime(endTime);
        var count = 0;

        for (var slot = startSlot; slot < endSlot; slot++)
        {
            if (IsAvailable(timeMap, slot))
                count++;
        }

        return count;
    }

    public static byte[] ToTimeMap(this IEnumerable<AvailableTimeSlot> availableTimeSlots)
    {
        var timeMap = new byte[12];
        foreach (var slot in availableTimeSlots)
        {
            SetAvailabilityRange(timeMap, slot.StartTime, slot.EndTime, true);
        }
        return timeMap;
    }
}