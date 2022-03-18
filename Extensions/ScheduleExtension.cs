using System;
using System.Collections.Generic;

static class ScheduleExtension
{
    public static bool OnTime(this DateTime dt, List<DayOfWeek> dayOfWeeks, int hour, int minute)
    {
        if (dayOfWeeks?.Count > 0)
        {
            return dayOfWeeks.Contains(dt.DayOfWeek) && ((hour == -1 || dt.Hour == hour) && (minute == -1 || dt.Minute == minute));
        }
        return false;
    }
}