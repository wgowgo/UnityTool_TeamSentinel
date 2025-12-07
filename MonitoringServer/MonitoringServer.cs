using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class MonitoringServer
{
    private TcpListener listener;
    private Dictionary<string, ClientState> clients = new();
    private Dictionary<string, DateTime> clientLastUpdate = new();
    private WebSocketServer wsServer = new();
    private ServerConfigManager configManager = new();
    private ServerConfig config;
    private bool isRunning = true;
    private object clientsLock = new object();

    public void Start()
    {
        config = configManager.LoadConfig();
        listener = new TcpListener(IPAddress.Loopback, config.TcpPort);
        listener.Start();
        wsServer.Start(config.WebSocketPort);
        Console.WriteLine($"[SERVER] Localhost Monitoring Server Running... (TCP:{config.TcpPort}, WS:{config.WebSocketPort})");
        
        Thread acceptThread = new Thread(AcceptLoop);
        acceptThread.Start();
        
        Thread cleanupThread = new Thread(CleanupLoop);
        cleanupThread.Start();
    }

    private void AcceptLoop()
    {
        while (isRunning)
        {
            try
            {
                TcpClient socket = listener.AcceptTcpClient();
                new Thread(() =>
                {
                    var handler = new TcpClientHandler(socket, this);
                    handler.Process();
                }).Start();
            }
            catch (Exception ex)
            {
                if (isRunning)
                {
                    LogError($"클라이언트 수락 오류: {ex.Message}");
                }
            }
        }
    }

    private void CleanupLoop()
    {
        while (isRunning)
        {
            try
            {
                Thread.Sleep(30000); // 30초마다 체크
                lock (clientsLock)
                {
                    var now = DateTime.Now;
                    var toRemove = new List<string>();
                    
                    foreach (var kv in clientLastUpdate.ToList())
                    {
                        if ((now - kv.Value).TotalMinutes > 5) // 5분 이상 업데이트 없으면 제거
                        {
                            toRemove.Add(kv.Key);
                        }
                    }
                    
                    foreach (var ip in toRemove)
                    {
                        clients.Remove(ip);
                        clientLastUpdate.Remove(ip);
                        Console.WriteLine($"[SERVER] 클라이언트 제거: {ip} (타임아웃)");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"클린업 오류: {ex.Message}");
            }
        }
    }

    public void UpdateClient(string ip, ClientState state)
    {
        lock (clientsLock)
        {
            clients[ip] = state;
            clientLastUpdate[ip] = DateTime.Now;
        }

        try
        {
            JsonLogWriter.Write(state);
            wsServer.Broadcast(state.ToJson());
            AlertEngine.Check(state);
            AnomalyDetector.Scan(state);
        }
        catch (Exception ex)
        {
            LogError($"클라이언트 업데이트 오류: {ex.Message}");
        }

        UpdateConsole();
    }

    public void RemoveClient(string ip)
    {
        lock (clientsLock)
        {
            clients.Remove(ip);
            clientLastUpdate.Remove(ip);
        }
        UpdateConsole();
    }

    private void UpdateConsole()
    {
        try
        {
            Console.Clear();
            Console.WriteLine("[SERVER] Unity Team Monitoring Server\n");
            lock (clientsLock)
            {
                foreach (var kv in clients)
                {
                    var s = kv.Value;
                    Console.WriteLine($"{s.PcName} → {s.Status} | CPU:{s.CpuUsage}% MEM:{s.MemoryUsage}% | Unity:{s.UnityCpuUsage}%/{s.UnityMemoryUsage}MB | Work:{s.TodayWorkMinutes}m");
                }
                if (clients.Count == 0)
                {
                    Console.WriteLine("연결된 클라이언트 없음");
                }
            }
        }
        catch (Exception ex)
        {
            LogError($"콘솔 업데이트 오류: {ex.Message}");
        }
    }

    private void LogError(string message)
    {
        try
        {
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            string logFile = Path.Combine(logDir, $"server_{DateTime.Now:yyyyMMdd}.log");
            System.IO.File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch { }
    }

    public void Stop()
    {
        isRunning = false;
        try { listener?.Stop(); } catch { }
        wsServer.Stop();
    }
}

