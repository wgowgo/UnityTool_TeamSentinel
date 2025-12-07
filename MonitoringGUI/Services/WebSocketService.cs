using System;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonitoringGUI
{
    public class WebSocketService
    {
        private ClientWebSocket socket;
        private CancellationTokenSource cts;
        public ObservableCollection<ClientStatusModel> Clients = new();
        public Action<ClientStatusModel> OnUpdate;

        public async void Start()
        {
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            socket = new ClientWebSocket();
            cts = new CancellationTokenSource();

            try
            {
                await socket.ConnectAsync(new Uri("ws://127.0.0.1:6000"), cts.Token);
                Task.Run(ReceiveLoop);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebSocket 연결 오류: {ex.Message}");
                // 재연결 시도
                await Task.Delay(3000);
                if (socket.State != WebSocketState.Open)
                {
                    await ConnectAsync();
                }
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (socket != null && socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        try
                        {
                            var state = JsonConvert.DeserializeObject<ClientStatusModel>(msg);
                            if (state != null)
                                OnUpdate?.Invoke(state);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"메시지 파싱 오류: {ex.Message}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException)
                {
                    // 연결 끊김, 재연결 시도
                    if (socket.State != WebSocketState.Open)
                    {
                        await Task.Delay(3000);
                        await ConnectAsync();
                    }
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"수신 오류: {ex.Message}");
                    break;
                }
            }
        }

        public void Stop()
        {
            cts?.Cancel();
            socket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }
}

