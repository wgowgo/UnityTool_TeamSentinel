using System;
using System.Net.Sockets;
using System.Text;

public class TcpClientHandler
{
    private TcpClient client;
    private MonitoringServer server;

    public TcpClientHandler(TcpClient c, MonitoringServer s)
    {
        client = c;
        server = s;
    }

    public void Process()
    {
        var stream = client.GetStream();
        var buffer = new byte[4096];
        string ip = null;

        try
        {
            ip = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        }
        catch
        {
            ip = "Unknown";
        }

        try
        {
            while (client.Connected)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, read);
                try
                {
                    ClientState state = ClientState.FromJson(json);
                    if (state != null)
                    {
                        server.UpdateClient(ip, state);
                    }
                }
                catch (Exception ex)
                {
                    // JSON 파싱 오류는 무시하고 계속 진행
                    System.Diagnostics.Debug.WriteLine($"JSON 파싱 오류: {ex.Message}");
                }
            }
        }
        catch (System.Net.Sockets.SocketException)
        {
            // 정상적인 연결 종료
        }
        catch (System.IO.IOException)
        {
            // 스트림 종료
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"클라이언트 처리 오류: {ex.Message}");
        }
        finally
        {
            try
            {
                server.RemoveClient(ip);
            }
            catch { }
            
            try
            {
                client.Close();
            }
            catch { }
        }
    }
}

