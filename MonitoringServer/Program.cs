using System;

class Program
{
    static void Main()
    {
        Console.Title = "Unity Monitoring Server (Localhost Only)";
        var server = new MonitoringServer();
        server.Start();
        Console.WriteLine("Press ENTER to stop server...");
        Console.ReadLine();
        server.Stop();
    }
}

