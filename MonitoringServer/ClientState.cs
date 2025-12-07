using Newtonsoft.Json;

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

