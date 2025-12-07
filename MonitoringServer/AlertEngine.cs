using System;

public static class AlertEngine
{
    public static void Check(ClientState state)
    {
        if (state.IdleMinutes > 30 && state.UnityRunning)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: Unity 실행 중이지만 30분 이상 유휴 상태");
        }

        if (!state.UnityRunning && state.IdleMinutes < 5)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: Unity 미실행 상태");
        }

        if (state.FocusLevel < 0.3 && state.UnityRunning)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: Unity 실행 중이지만 집중도 낮음");
        }

        if (state.CpuUsage > 90)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: CPU 사용률 높음 ({state.CpuUsage}%)");
        }

        if (state.MemoryUsage > 90)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: 메모리 사용률 높음 ({state.MemoryUsage}%)");
        }

        if (state.UnityCpuUsage > 80)
        {
            Console.WriteLine($"[ALERT] {state.PcName}: Unity CPU 사용률 높음 ({state.UnityCpuUsage}%)");
        }
    }
}

