using System;

namespace MonitoringGUI
{
    public class LogEntryModel
    {
        public DateTime timestamp { get; set; }
        public string PcName { get; set; }
        public string Status { get; set; }
        public bool UnityRunning { get; set; }
        public int IdleMinutes { get; set; }
        public double FocusLevel { get; set; }
        public string ActiveWindow { get; set; }
        public string[] Events { get; set; }
    }
}

