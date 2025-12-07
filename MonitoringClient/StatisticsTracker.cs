using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class StatisticsTracker
{
    private string statsDir;
    private Dictionary<string, DailyStats> dailyStats;

    public StatisticsTracker()
    {
        statsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnityMonitoring", "Statistics");
        Directory.CreateDirectory(statsDir);
        dailyStats = new Dictionary<string, DailyStats>();
        LoadTodayStats();
    }

    public void UpdateStats(bool unityRunning, int idleMinutes, double cpuUsage, double memoryUsage)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (!dailyStats.ContainsKey(today))
        {
            dailyStats[today] = new DailyStats { Date = today };
        }

        var stats = dailyStats[today];
        stats.TotalMinutes++;

        if (unityRunning && idleMinutes < 5)
        {
            stats.UnityWorkMinutes++;
        }

        if (idleMinutes > 30)
        {
            stats.IdleMinutes++;
        }

        stats.AvgCpuUsage = (stats.AvgCpuUsage * (stats.TotalMinutes - 1) + cpuUsage) / stats.TotalMinutes;
        stats.AvgMemoryUsage = (stats.AvgMemoryUsage * (stats.TotalMinutes - 1) + memoryUsage) / stats.TotalMinutes;
        stats.MaxCpuUsage = Math.Max(stats.MaxCpuUsage, cpuUsage);
        stats.MaxMemoryUsage = Math.Max(stats.MaxMemoryUsage, memoryUsage);

        SaveStats(today, stats);
    }

    public DailyStats GetTodayStats()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (dailyStats.ContainsKey(today))
            return dailyStats[today];
        return new DailyStats { Date = today };
    }

    public DailyStats[] GetWeeklyStats()
    {
        var stats = new List<DailyStats>();
        for (int i = 6; i >= 0; i--)
        {
            string date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
            if (dailyStats.ContainsKey(date))
                stats.Add(dailyStats[date]);
            else
                stats.Add(new DailyStats { Date = date });
        }
        return stats.ToArray();
    }

    private void LoadTodayStats()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string filepath = Path.Combine(statsDir, $"{today}.json");
        if (File.Exists(filepath))
        {
            try
            {
                string json = File.ReadAllText(filepath);
                dailyStats[today] = JsonConvert.DeserializeObject<DailyStats>(json);
            }
            catch { }
        }
    }

    private void SaveStats(string date, DailyStats stats)
    {
        string filepath = Path.Combine(statsDir, $"{date}.json");
        try
        {
            string json = JsonConvert.SerializeObject(stats, Formatting.Indented);
            File.WriteAllText(filepath, json);
        }
        catch { }
    }
}

public class DailyStats
{
    public string Date;
    public int TotalMinutes;
    public int UnityWorkMinutes;
    public int IdleMinutes;
    public double AvgCpuUsage;
    public double AvgMemoryUsage;
    public double MaxCpuUsage;
    public double MaxMemoryUsage;
}

