using System;
using System.Collections.Generic;
using System.Text;

public static class DatetimeExtensions
{
    public static ulong DatetimeTicksLong(this DateTime? datetime)
    {
        return datetime.HasValue ? (ulong)datetime.Value.Ticks : 0;
    }

    public static ulong DatetimeTicksLong(this DateTime datetime)
    {
        return (ulong)datetime.Ticks;
    }

    public static DateTime? TicksLongDatetime(this ulong ticks)
    {
        if (ticks == 0)
            return null;
        return new DateTime((long)ticks);
    }

    public static DateTime TicksLongDatetimeNotNull(this ulong ticks)
    {
        if (ticks == 0)
            return DateTime.MinValue;
        return new DateTime((long)ticks);
    }
}