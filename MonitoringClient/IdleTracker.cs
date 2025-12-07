using System;
using System.Runtime.InteropServices;

public class IdleTracker
{
    [DllImport("User32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    public int GetIdleMinutes()
    {
        LASTINPUTINFO info = new LASTINPUTINFO();
        info.cbSize = (uint)Marshal.SizeOf(info);
        if (!GetLastInputInfo(ref info)) return 0;
        uint idleTime = ((uint)Environment.TickCount - info.dwTime) / 60000;
        return (int)idleTime;
    }

    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}

