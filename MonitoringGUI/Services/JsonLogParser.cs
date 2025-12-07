using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonitoringGUI
{
    public static class JsonLogParser
    {
        public static List<LogEntryModel> ReadLogs(string date)
        {
            string path = $"logs/{date}.json";
            var entries = new List<LogEntryModel>();

            if (!File.Exists(path))
                return entries;

            foreach (string line in File.ReadAllLines(path))
            {
                try
                {
                    var entry = JsonConvert.DeserializeObject<LogEntryModel>(line);
                    if (entry != null)
                        entries.Add(entry);
                }
                catch { }
            }

            return entries;
        }
    }
}

