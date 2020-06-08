using System;
using System.Collections.Generic;
using System.Text;

public static class DatetimeExtensions
{
    public static ulong DatetimeTicksLong(this DateTime? datetime)
    {
        return datetime.HasValue ? (ulong)datetime.Value.Ticks : 0;
    }

    public static DateTime? TicksLongDatetime(this ulong ticks)
    {
        if (ticks == 0)
            return null;
        return new DateTime((long)ticks);
    }
}