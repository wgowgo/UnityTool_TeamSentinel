using System;

class Program
{
    static MonitoringClient client;

    static void Main()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            client?.Stop();
            Environment.Exit(0);
        };

        client = new MonitoringClient();
        client.Start();
        
        // 메인 스레드 유지
        Console.WriteLine("모니터링 클라이언트가 실행 중입니다. 종료하려면 Ctrl+C를 누르세요.");
        while (true)
        {
            System.Threading.Thread.Sleep(1000);
        }
    }
}

