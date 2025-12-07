using Fleck;
using System.Collections.Generic;
using System.Linq;

public class WebSocketServer
{
    private List<IWebSocketConnection> sockets = new();

    public void Start(int port)
    {
        var server = new Fleck.WebSocketServer($"ws://127.0.0.1:{port}");
        server.Start(sock =>
        {
            sock.OnOpen = () => sockets.Add(sock);
            sock.OnClose = () => sockets.Remove(sock);
        });
        Console.WriteLine("[WS] Running on ws://127.0.0.1:6000");
    }

    public void Broadcast(string msg)
    {
        foreach (var s in sockets)
            s.Send(msg);
    }

    public void Stop()
    {
        try
        {
            foreach (var socket in sockets.ToList())
            {
                try
                {
                    socket.Close();
                }
                catch { }
            }
            sockets.Clear();
        }
        catch { }
    }
}

