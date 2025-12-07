using System;
using System.IO;
using Newtonsoft.Json;

public class ServerConfigManager
{
    private string configPath;

    public ServerConfigManager()
    {
        configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public ServerConfig LoadConfig()
    {
        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<ServerConfig>(json);
                if (ValidateConfig(config))
                {
                    return config;
                }
                else
                {
                    Console.WriteLine("[경고] 설정 파일에 잘못된 값이 있습니다. 기본값을 사용합니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[오류] 설정 파일 로드 실패: {ex.Message}");
            }
        }

        var defaultConfig = new ServerConfig
        {
            TcpPort = 5000,
            WebSocketPort = 6000,
            LogDirectory = "logs",
            AlertThresholdIdleMinutes = 30,
            AlertThresholdCpuUsage = 90,
            AlertThresholdMemoryUsage = 90
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private bool ValidateConfig(ServerConfig config)
    {
        if (config == null) return false;
        
        // 포트 범위 검증 (1-65535)
        if (config.TcpPort < 1 || config.TcpPort > 65535)
            return false;
        
        if (config.WebSocketPort < 1 || config.WebSocketPort > 65535)
            return false;
        
        // TCP와 WebSocket 포트가 같으면 안됨
        if (config.TcpPort == config.WebSocketPort)
            return false;
        
        // 로그 디렉토리 검증
        if (string.IsNullOrWhiteSpace(config.LogDirectory))
            return false;
        
        // 임계값 검증
        if (config.AlertThresholdIdleMinutes < 0)
            return false;
        
        if (config.AlertThresholdCpuUsage < 0 || config.AlertThresholdCpuUsage > 100)
            return false;
        
        if (config.AlertThresholdMemoryUsage < 0 || config.AlertThresholdMemoryUsage > 100)
            return false;
        
        return true;
    }

    public void SaveConfig(ServerConfig config)
    {
        try
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }
        catch { }
    }
}

public class ServerConfig
{
    public int TcpPort;
    public int WebSocketPort;
    public string LogDirectory;
    public int AlertThresholdIdleMinutes;
    public double AlertThresholdCpuUsage;
    public double AlertThresholdMemoryUsage;
}

