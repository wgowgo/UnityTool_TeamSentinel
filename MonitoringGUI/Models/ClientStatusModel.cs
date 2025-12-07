namespace MonitoringGUI
{
    public class ClientStatusModel
    {
        public string PcName { get; set; }
        public string Status { get; set; }
        public bool UnityRunning { get; set; }
        public int IdleMinutes { get; set; }
        public double FocusLevel { get; set; }
        public string ActiveWindow { get; set; }
        public string[] Events { get; set; }
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double UnityCpuUsage { get; set; }
        public double UnityMemoryUsage { get; set; }
        public int TodayWorkMinutes { get; set; }
        public string ScreenshotPath { get; set; }
    }
}

