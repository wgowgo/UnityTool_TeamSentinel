using System;

public static class AnomalyDetector
{
    public static void Scan(ClientState state)
    {
        if (state.Events != null && state.Events.Length > 10)
        {
            Console.WriteLine($"[ANOMALY] {state.PcName}: 파일 삭제 이벤트 과다 ({state.Events.Length}건)");
        }

        if (state.IdleMinutes > 120)
        {
            Console.WriteLine($"[ANOMALY] {state.PcName}: 2시간 이상 유휴 상태");
        }

        if (state.UnityRunning && state.FocusLevel < 0.1)
        {
            Console.WriteLine($"[ANOMALY] {state.PcName}: Unity 실행 중이지만 집중도 매우 낮음");
        }
    }
}

