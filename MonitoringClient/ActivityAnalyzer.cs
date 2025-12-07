using System;
using System.Linq;
using Newtonsoft.Json;

public class ActivityAnalyzer
{
    public ClientState BuildState(bool unityRunning, int idleMinutes, double focusLevel, string activeWindow, string[] events, 
        double cpuUsage, double memoryUsage, double unityCpuUsage, double unityMemoryUsage, DailyStats todayStats)
    {
        string status = "정상";
        if (!unityRunning)
            status = "Unity 미실행";
        else if (idleMinutes > 30)
            status = "유휴 상태";
        else if (focusLevel < 0.3)
            status = "집중도 낮음";

        return new ClientState
        {
            PcName = Environment.MachineName,
            Status = status,
            UnityRunning = unityRunning,
            IdleMinutes = idleMinutes,
            FocusLevel = focusLevel,
            ActiveWindow = activeWindow,
            Events = events ?? new string[0],
            CpuUsage = cpuUsage,
            MemoryUsage = memoryUsage,
            UnityCpuUsage = unityCpuUsage,
            UnityMemoryUsage = unityMemoryUsage,
            TodayWorkMinutes = todayStats?.UnityWorkMinutes ?? 0,
            TodayTotalMinutes = todayStats?.TotalMinutes ?? 0,
            ScreenshotPath = null // MonitoringClient에서 설정됨
        };
    }
}

public class ClientState
{
    public string PcName;
    public string Status;
    public bool UnityRunning;
    public int IdleMinutes;
    public double FocusLevel;
    public string ActiveWindow;
    public string[] Events;
    public double CpuUsage;
    public double MemoryUsage;
    public double UnityCpuUsage;
    public double UnityMemoryUsage;
    public int TodayWorkMinutes;
    public int TodayTotalMinutes;
    public string ScreenshotPath;

    public string ToJson() => JsonConvert.SerializeObject(this);
    
    public static ClientState FromJson(string json) => 
        JsonConvert.DeserializeObject<ClientState>(json);
}

