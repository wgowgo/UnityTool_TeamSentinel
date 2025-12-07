using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class MonitoringClient
{
    private TcpClient tcp;
    private NetworkStream stream;
    private bool isRunning = true;
    private object streamLock = new object();
    UnityWatcher unity = new();
    IdleTracker idle = new();
    FileDeletionWatcher fileWatcher = new();
    FocusMonitor focus = new();
    ActivityAnalyzer analyzer = new();
    ResourceMonitor resourceMonitor = new();
    StatisticsTracker statisticsTracker = new();
    ScreenshotCapture screenshotCapture = new();
    ConfigManager configManager = new();
    private ClientConfig config;

    public void Start()
    {
        config = configManager.LoadConfig();
        fileWatcher.Start();
        new Thread(SendLoop).Start();
    }

    private bool ConnectToServer()
    {
        try
        {
            if (tcp != null)
            {
                try { tcp.Close(); } catch { }
            }

            tcp = new TcpClient();
            tcp.Connect(config.ServerAddress, config.ServerPort);
            stream = tcp.GetStream();
            LogError($"서버에 연결되었습니다: {config.ServerAddress}:{config.ServerPort}");
            return true;
        }
        catch (Exception ex)
        {
            LogError($"서버 연결 실패: {ex.Message}");
            return false;
        }
    }

    private void SendLoop()
    {
        bool lastAnomalyState = false;
        string lastScreenshotPath = null;

        while (isRunning)
        {
            try
            {
                // 연결 확인 및 재연결
                if (tcp == null || !tcp.Connected || stream == null)
                {
                    if (!ConnectToServer())
                    {
                        Thread.Sleep(5000); // 5초 후 재시도
                        continue;
                    }
                }

                bool unityRunning = unity.IsUnityRunning();
                int idleMinutes = idle.GetIdleMinutes();
                double cpuUsage = resourceMonitor.GetCpuUsage();
                double memoryUsage = resourceMonitor.GetMemoryUsage();
                double unityCpuUsage = resourceMonitor.GetUnityCpuUsage();
                double unityMemoryUsage = resourceMonitor.GetUnityMemoryUsage();

                bool isAnomaly = (idleMinutes > 30 && unityRunning) || (cpuUsage > 90) || (memoryUsage > 90);
                string screenshotPath = null;

                if (isAnomaly && !lastAnomalyState && config.ScreenshotOnAnomaly)
                {
                    screenshotPath = screenshotCapture.CaptureScreenshot();
                    if (!string.IsNullOrEmpty(screenshotPath))
                    {
                        lastScreenshotPath = screenshotPath;
                    }
                }
                else if (!isAnomaly)
                {
                    lastScreenshotPath = null;
                }

                lastAnomalyState = isAnomaly;

                statisticsTracker.UpdateStats(unityRunning, idleMinutes, cpuUsage, memoryUsage);
                var todayStats = statisticsTracker.GetTodayStats();

                ClientState state = analyzer.BuildState(
                    unityRunning,
                    idleMinutes,
                    focus.GetFocusLevel(),
                    focus.GetActiveWindow(),
                    fileWatcher.GetEvents(),
                    cpuUsage,
                    memoryUsage,
                    unityCpuUsage,
                    unityMemoryUsage,
                    todayStats
                );

                // 스크린샷 경로 포함
                state.ScreenshotPath = lastScreenshotPath;

                // 데이터 전송
                byte[] data = Encoding.UTF8.GetBytes(state.ToJson());
                lock (streamLock)
                {
                    if (stream != null && tcp != null && tcp.Connected)
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }

                Thread.Sleep(config.UpdateInterval);
            }
            catch (SocketException ex)
            {
                LogError($"네트워크 오류: {ex.Message}");
                try { if (tcp != null) tcp.Close(); } catch { }
                tcp = null;
                stream = null;
                Thread.Sleep(5000); // 재연결 대기
            }
            catch (IOException ex)
            {
                LogError($"스트림 오류: {ex.Message}");
                try { if (tcp != null) tcp.Close(); } catch { }
                tcp = null;
                stream = null;
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                LogError($"예상치 못한 오류: {ex.Message}");
                Thread.Sleep(3000);
            }
        }
    }

    private void LogError(string message)
    {
        try
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnityMonitoring", "Logs");
            Directory.CreateDirectory(logDir);
            string logFile = Path.Combine(logDir, $"client_{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch { }
    }

    public void Stop()
    {
        isRunning = false;
        try { if (stream != null) stream.Close(); } catch { }
        try { if (tcp != null) tcp.Close(); } catch { }
    }
}

