using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public static class JsonLogWriter
{
    private static string logDirectory = "logs";
    private static DateTime lastCleanup = DateTime.MinValue;

    public static void Write(ClientState state)
    {
        try
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            Directory.CreateDirectory(logDirectory);
            string path = Path.Combine(logDirectory, $"{date}.json");

            var log = new
            {
                timestamp = DateTime.Now,
                state.PcName,
                state.Status,
                state.UnityRunning,
                state.IdleMinutes,
                state.FocusLevel,
                state.ActiveWindow,
                state.Events,
                state.CpuUsage,
                state.MemoryUsage,
                state.UnityCpuUsage,
                state.UnityMemoryUsage,
                state.TodayWorkMinutes
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(log) + "\n");

            // 1시간마다 오래된 로그 정리
            if ((DateTime.Now - lastCleanup).TotalHours >= 1)
            {
                CleanupOldLogs();
                lastCleanup = DateTime.Now;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"로그 작성 오류: {ex.Message}");
        }
    }

    private static void CleanupOldLogs()
    {
        try
        {
            if (!Directory.Exists(logDirectory))
                return;

            var files = Directory.GetFiles(logDirectory, "*.json")
                .Select(f => new FileInfo(f))
                .Where(f => (DateTime.Now - f.LastWriteTime).TotalDays > 30) // 30일 이상 된 로그 삭제
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    file.Delete();
                }
                catch { }
            }

            // 로그 파일 크기 제한 (100MB 이상이면 압축 또는 삭제)
            var allFiles = Directory.GetFiles(logDirectory, "*.json")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();

            long totalSize = allFiles.Sum(f => f.Length);
            const long maxSize = 100 * 1024 * 1024; // 100MB

            if (totalSize > maxSize)
            {
                // 오래된 파일부터 삭제
                foreach (var file in allFiles.Skip(30)) // 최근 30개 파일은 유지
                {
                    if (totalSize <= maxSize) break;
                    try
                    {
                        totalSize -= file.Length;
                        file.Delete();
                    }
                    catch { }
                }
            }
        }
        catch { }
    }
}

