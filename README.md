# 📘 Unity Team Sentinel
## 🇰🇷 팀 모니터링 시스템 · 🇺🇸 Team Monitoring System

### Unity 개발 팀의 작업 상태를 실시간으로 모니터링하여 **서버 / 클라이언트 / GUI / 웹 대시보드**에서 <br/> 확인할 수 있게 해주는 통합 모니터링 도구입니다.  
### This integrated monitoring tool allows real-time tracking of Unity development team activity, visible in **Server / Client / GUI / Web Dashboard**.


---

# ✨ 주요 기능 · Features

## 🇰🇷 한국어
- Unity 프로세스 실행 상태 실시간 감지  
- CPU 및 메모리 사용률 모니터링  
- 유휴 시간 및 집중도 측정  
- 이상 상태 자동 감지 및 알림  
- 일일 작업 시간 통계 수집  
- 스크린샷 자동 캡처 (이상 상태 발생 시)  
- 다중 클라이언트 동시 모니터링  
- 웹 기반 실시간 대시보드  

## 🇺🇸 English
- Real-time Unity process detection  
- CPU and memory usage monitoring  
- Idle time and focus level measurement  
- Automatic anomaly detection and alerts  
- Daily work time statistics collection  
- Automatic screenshot capture (on anomaly)  
- Multi-client simultaneous monitoring  
- Web-based real-time dashboard  

---

# 📂 폴더 구조 · Folder Structure

UNITY_TEAM_SENTINEL/<br/>
├── MonitoringClient/                    ← 클라이언트 애플리케이션/<br/>
│   ├── MonitoringClient.cs              ← 메인 클라이언트 로직/<br/>
│   ├── ResourceMonitor.cs               ← 리소스 모니터링/<br/>
│   ├── UnityWatcher.cs                 ← Unity 프로세스 감지/<br/>
│   ├── IdleTracker.cs                  ← 유휴 시간 추적/<br/>
│   ├── FocusMonitor.cs                 ← 집중도 측정/<br/>
│   ├── StatisticsTracker.cs            ← 통계 수집/<br/>
│   ├── ScreenshotCapture.cs            ← 스크린샷 캡처/<br/>
│   └── ConfigManager.cs                ← 설정 관리/<br/>
├── MonitoringServer/                    ← 서버 애플리케이션/<br/>
│   ├── MonitoringServer.cs             ← 메인 서버 로직/<br/>
│   ├── TcpClientHandler.cs             ← TCP 클라이언트 처리/<br/>
│   ├── WebSocketServer.cs              ← WebSocket 서버/<br/>
│   ├── AlertEngine.cs                  ← 알림 엔진/<br/>
│   ├── AnomalyDetector.cs              ← 이상 감지/<br/>
│   ├── JsonLogWriter.cs                ← 로그 기록/<br/>
│   └── ServerConfigManager.cs          ← 서버 설정 관리/<br/>
├── MonitoringGUI/                       ← GUI 애플리케이션/<br/>
│   ├── MainWindow.xaml                 ← 메인 윈도우 UI/<br/>
│   ├── Services/                       ← 서비스 레이어/<br/>
│   │   └── WebSocketService.cs         ← WebSocket 클라이언트/<br/>
│   └── Models/                         ← 데이터 모델/<br/>
│       └── ClientStatusModel.cs        ← 클라이언트 상태 모델/<br/>
├── DashboardWeb/                        ← 웹 대시보드/<br/>
│   ├── index.html                      ← 메인 HTML/<br/>
│   ├── dashboard.css                   ← 스타일시트/<br/>
│   └── ws-client.js                    ← WebSocket 클라이언트/<br/>
└── publish/                             ← 배포용 빌드 출력/<br/>
    ├── Client/                         ← 클라이언트 EXE/<br/>
    ├── Server/                         ← 서버 EXE/<br/>
    └── GUI/                            ← GUI EXE/<br/>


---

# 🚀 사용 방법 · How to Use

## 🇰🇷 한국어

### 1) 서버 실행
- `publish\Server\MonitoringServer.exe` 실행  
- 콘솔 창에서 TCP(5000), WebSocket(6000) 포트 확인  
- 클라이언트 연결 대기 상태 표시됨  

### 2) 클라이언트 실행
- 각 개발자 PC에서 `publish\Client\MonitoringClient.exe` 실행  
- 서버에 자동 연결 시도, 연결 실패 시 5초마다 재시도  
- 시스템 상태 수집 및 전송 시작  

### 3) 모니터링
**GUI 애플리케이션:**
- `publish\GUI\MonitoringGUI.exe` 실행  
- 실시간 클라이언트 상태 표시  
- CPU/메모리 사용률, Unity 실행 상태 확인  

**웹 대시보드:**
- `DashboardWeb\index.html` 브라우저에서 열기  
- 실시간 차트 및 통계 확인  
- 다중 클라이언트 동시 모니터링  

### 4) 설정 변경
- 서버: `publish\Server\appsettings.json` 수정  
- 클라이언트: 실행 시 자동 생성되는 `appsettings.json` 수정  
- 설정 변경 후 재시작 필요  

---

## 🇺🇸 English

### 1) Start Server
- Run `publish\Server\MonitoringServer.exe`  
- Check TCP(5000) and WebSocket(6000) ports in console  
- Server waits for client connections  

### 2) Start Client
- Run `publish\Client\MonitoringClient.exe` on each developer PC  
- Automatically attempts to connect to server, retries every 5 seconds if failed  
- Begins collecting and sending system status  

### 3) Monitor
**GUI Application:**
- Run `publish\GUI\MonitoringGUI.exe`  
- View real-time client status  
- Check CPU/memory usage, Unity running status  

**Web Dashboard:**
- Open `DashboardWeb\index.html` in browser  
- View real-time charts and statistics  
- Monitor multiple clients simultaneously  

### 4) Change Settings
- Server: Edit `publish\Server\appsettings.json`  
- Client: Edit `appsettings.json` (auto-generated on first run)  
- Restart required after configuration changes  

---

# ⚙️ 설정 · Configuration

## 🇰🇷 한국어

### 서버 설정 (appsettings.json)
```json
{
  "TcpPort": 5000,
  "WebSocketPort": 6000,
  "LogDirectory": "logs",
  "AlertThresholdIdleMinutes": 30,
  "AlertThresholdCpuUsage": 90.0,
  "AlertThresholdMemoryUsage": 90.0
}
```

### 클라이언트 설정 (appsettings.json)
```json
{
  "ServerAddress": "127.0.0.1",
  "ServerPort": 5000,
  "UpdateInterval": 3000,
  "EnableScreenshots": true,
  "ScreenshotOnAnomaly": true
}
```

---

## 🇺🇸 English

### Server Configuration (appsettings.json)
```json
{
  "TcpPort": 5000,
  "WebSocketPort": 6000,
  "LogDirectory": "logs",
  "AlertThresholdIdleMinutes": 30,
  "AlertThresholdCpuUsage": 90.0,
  "AlertThresholdMemoryUsage": 90.0
}
```

### Client Configuration (appsettings.json)
```json
{
  "ServerAddress": "127.0.0.1",
  "ServerPort": 5000,
  "UpdateInterval": 3000,
  "EnableScreenshots": true,
  "ScreenshotOnAnomaly": true
}
```
---

## 🇺🇸 English

### Deployment Build (Single EXE)
```powershell
# MonitoringClient
cd MonitoringClient
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ..\publish\Client

# MonitoringServer
cd ..\MonitoringServer
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ..\publish\Server

# MonitoringGUI
cd ..\MonitoringGUI
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ..\publish\GUI
```

---

# ⚠ 주의 사항 · Notes

## 🇰🇷 한국어
- 서버는 **TCP 5000**, **WebSocket 6000** 포트 사용  
- 클라이언트는 서버 연결 실패 시 **자동 재연결** 시도  
- 스크린샷은 `%USERPROFILE%\Documents\UnityMonitoring\Screenshots\`에 저장  
- 통계 파일은 `%USERPROFILE%\Documents\UnityMonitoring\Statistics\`에 저장  
- 로그 파일은 `publish\Server\logs\`에 저장 (30일 자동 삭제)  
- 배포용 EXE는 **.NET 런타임 포함** (별도 설치 불필요)  
- 현재 버전은 **localhost 전용** (프로덕션 환경에서는 보안 강화 권장)  

## 🇺🇸 English
- Server uses **TCP 5000** and **WebSocket 6000** ports  
- Client **automatically reconnects** if server connection fails  
- Screenshots saved to `%USERPROFILE%\Documents\UnityMonitoring\Screenshots\`  
- Statistics saved to `%USERPROFILE%\Documents\UnityMonitoring\Statistics\`  
- Log files saved to `publish\Server\logs\` (auto-deleted after 30 days)  
- Deployment EXE includes **.NET runtime** (no separate installation needed)  
- Current version is **localhost only** (security hardening recommended for production)  

---
<img width="1213" height="239" alt="image" src="https://github.com/user-attachments/assets/aed25eb6-9917-41e5-af00-14e6c269d3c1" />
<img width="975" height="160" alt="image" src="https://github.com/user-attachments/assets/04780e9f-16a7-4276-8d59-045585a1403d" />

