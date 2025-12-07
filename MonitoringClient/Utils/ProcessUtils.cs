using System.Diagnostics;

public static class ProcessUtils
{
    public static bool IsProcessRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Length > 0;
    }

    public static int GetProcessCount(string processName)
    {
        return Process.GetProcessesByName(processName).Length;
    }
}

