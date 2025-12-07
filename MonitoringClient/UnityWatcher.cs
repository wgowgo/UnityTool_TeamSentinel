using System.Diagnostics;

public class UnityWatcher
{
    public bool IsUnityRunning()
    {
        return Process.GetProcessesByName("Unity").Length > 0;
    }
}

