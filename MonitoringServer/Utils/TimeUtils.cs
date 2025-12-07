using System;

public static class TimeUtils
{
    public static string FormatDuration(int minutes)
    {
        if (minutes < 60)
            return $"{minutes}분";
        int hours = minutes / 60;
        int mins = minutes % 60;
        return $"{hours}시간 {mins}분";
    }

    public static DateTime ParseTimestamp(string timestamp)
    {
        return DateTime.Parse(timestamp);
    }
}

