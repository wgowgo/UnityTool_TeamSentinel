using System;
using System.Diagnostics;
using System.Management;
using System.Threading;

public class ResourceMonitor
{
    private PerformanceCounter cpuCounter;
    private PerformanceCounter ramCounter;
    private PerformanceCounter unityCpuCounter;
    private PerformanceCounter unityRamCounter;

    public ResourceMonitor()
    {
        try
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            cpuCounter.NextValue();
            ramCounter.NextValue();
        }
        catch { }
    }

    public double GetCpuUsage()
    {
        try
        {
            if (cpuCounter != null)
                return Math.Round(cpuCounter.NextValue(), 2);
        }
        catch { }
        return 0.0;
    }

    public double GetMemoryUsage()
    {
        try
        {
            if (ramCounter != null)
            {
                double availableMB = ramCounter.NextValue();
                double totalMB = GetTotalMemoryMB();
                double usedMB = totalMB - availableMB;
                return Math.Round((usedMB / totalMB) * 100, 2);
            }
        }
        catch { }
        return 0.0;
    }

    public double GetUnityCpuUsage()
    {
        try
        {
            Process[] unityProcs = Process.GetProcessesByName("Unity");
            if (unityProcs.Length > 0)
            {
                double totalCpu = 0;
                foreach (var proc in unityProcs)
                {
                    try
                    {
                        using (var pc = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName))
                        {
                            pc.NextValue();
                            Thread.Sleep(100);
                            totalCpu += pc.NextValue();
                        }
                    }
                    catch { }
                }
                return Math.Round(totalCpu / Environment.ProcessorCount, 2);
            }
        }
        catch { }
        return 0.0;
    }

    public double GetUnityMemoryUsage()
    {
        try
        {
            Process[] unityProcs = Process.GetProcessesByName("Unity");
            long totalMemory = 0;
            foreach (var proc in unityProcs)
            {
                try
                {
                    totalMemory += proc.WorkingSet64;
                }
                catch { }
            }
            return Math.Round(totalMemory / (1024.0 * 1024.0), 2);
        }
        catch { }
        return 0.0;
    }

    private double GetTotalMemoryMB()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024);
                }
            }
        }
        catch { }
        return 8192;
    }
}

