using System;
using System.IO;
using Newtonsoft.Json;

public class ConfigManager
{
    private string configPath;

    public ConfigManager()
    {
        configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public ClientConfig LoadConfig()
    {
        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<ClientConfig>(json);
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

        var defaultConfig = new ClientConfig
        {
            ServerAddress = "127.0.0.1",
            ServerPort = 5000,
            UpdateInterval = 3000,
            EnableScreenshots = true,
            ScreenshotOnAnomaly = true
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private bool ValidateConfig(ClientConfig config)
    {
        if (config == null) return false;
        
        // 서버 주소 검증
        if (string.IsNullOrWhiteSpace(config.ServerAddress))
            return false;
        
        // 포트 범위 검증 (1-65535)
        if (config.ServerPort < 1 || config.ServerPort > 65535)
            return false;
        
        // 업데이트 간격 검증 (최소 1초)
        if (config.UpdateInterval < 1000)
            return false;
        
        return true;
    }

    public void SaveConfig(ClientConfig config)
    {
        try
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }
        catch { }
    }
}

public class ClientConfig
{
    public string ServerAddress;
    public int ServerPort;
    public int UpdateInterval;
    public bool EnableScreenshots;
    public bool ScreenshotOnAnomaly;
}

