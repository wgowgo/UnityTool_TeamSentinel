using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public class FocusMonitor
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    public double GetFocusLevel()
    {
        IntPtr hwnd = GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return 0.0;

        uint processId;
        GetWindowThreadProcessId(hwnd, out processId);

        try
        {
            Process proc = Process.GetProcessById((int)processId);
            if (proc.ProcessName.ToLower().Contains("unity"))
                return 1.0;
            if (proc.ProcessName.ToLower().Contains("code") || proc.ProcessName.ToLower().Contains("visual"))
                return 0.7;
            return 0.3;
        }
        catch
        {
            return 0.0;
        }
    }

    public string GetActiveWindow()
    {
        IntPtr hwnd = GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return "Unknown";

        StringBuilder title = new StringBuilder(256);
        GetWindowText(hwnd, title, title.Capacity);
        return title.ToString();
    }
}

