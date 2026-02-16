using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Spectre.Console;
using Task_Manager_T4;

class Load
{
    public static void MainMenu()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]SYSTEM LOAD MONITORING[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

        var loadChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]Выберите действие:[/]")
                .AddChoices(
                [
                    "Показать текущую загрузку системы",
                    "Сохранить отчет о загрузке в файл",
                    "Назад"
                ]));

        switch (loadChoice)
        {
            case "Показать текущую загрузку системы":
                ShowSystemLoad();
                break;
            case "Сохранить отчет о загрузке в файл":
                SaveSystemLoadToFile();
                break;
            default:
                return;
        }
    }
    private static void ShowSystemLoad()
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]System Load Monitoring[/]");
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Updates every two seconds. Press any key to exit.[/]");

        var table = new Table
        {
            Border = TableBorder.Rounded
        };

        table.AddColumn(new TableColumn("[bold]Time[/]").Centered());
        table.AddColumn(new TableColumn("[bold]CPU %[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Memory %[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Memory (bytes)[/]").Centered());
        table.AddColumn(new TableColumn("[bold]GTD / Max Memory (bytes)[/]").Centered());
        table.AddColumn(new TableColumn("[bold]GTD / Max CPU (units)[/]").Centered());

        PerformanceCounter performanceCounterCpu = null;
        PerformanceCounter performanceCounterMemory = null;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                performanceCounterCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                performanceCounterMemory = new PerformanceCounter("Memory", "% Committed Bytes In Use");

                performanceCounterCpu.NextValue();
                performanceCounterMemory.NextValue();
                Thread.Sleep(1000);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Performance counters are only available on Windows.[/]");
                AnsiConsole.MarkupLine($"[{GraphicSettings.NeutralColor}]Using simulated data for demonstration.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error initializing performance counters: {ex.Message}[/]");
            AnsiConsole.MarkupLine($"[{GraphicSettings.NeutralColor}]Using simulated data for demonstration.[/]");
        }

        Random rand = new();
        long totalSystemMemory = GetTotalSystemMemory();

        while (!Console.KeyAvailable)
        {
            float cpuUsage;
            float memoryUsagePercent;
            long usedMemoryBytes;

            if (performanceCounterCpu != null && performanceCounterMemory != null)
            {
                cpuUsage = performanceCounterCpu.NextValue();

                memoryUsagePercent = performanceCounterMemory.NextValue();

                usedMemoryBytes = (long)(memoryUsagePercent / 100.0 * totalSystemMemory);
            }
            else
            {
                cpuUsage = rand.Next(0, 100);
                memoryUsagePercent = rand.Next(10, 80);
                usedMemoryBytes = (long)(memoryUsagePercent / 100.0 * totalSystemMemory);
            }
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            string cpuFormatted = $"{cpuUsage:F2} %";
            string memoryPercentFormatted = $"{memoryUsagePercent:F2} %";
            string memoryBytesFormatted = $"{usedMemoryBytes:N0}";
            string memoryMaxFormatted = $"{usedMemoryBytes:N0} / {totalSystemMemory:N0}";
            string cpuMaxFormatted = GetCpuGuaranteedMaxInfo();

            table.AddRow(
                currentTime,
                cpuFormatted,
                memoryPercentFormatted,
                memoryBytesFormatted,
                memoryMaxFormatted,
                cpuMaxFormatted
            );

            Console.Clear();
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]System Load Monitoring[/]");

            if (performanceCounterCpu == null || performanceCounterMemory == null)
            {
                AnsiConsole.MarkupLine("[yellow]Using simulated data[/]");
            }
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Updates every two seconds. Press any key to exit.[/]");
            AnsiConsole.Write(table.BorderColor(GraphicSettings.GetThemeColor)); //dodelat' //22:32:40 sdelal

            Thread.Sleep(2000);
        }
        while (Console.KeyAvailable) Console.ReadKey(true);

        performanceCounterCpu?.Dispose();
        performanceCounterMemory?.Dispose();
    }

    private static void SaveSystemLoadToFile()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "SystemReport");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string reportFile = Path.Combine(folderPath, $"systemload_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            using (StreamWriter sw = new(reportFile))
            {
                sw.WriteLine("=== SYSTEM LOAD REPORT ===");
                sw.WriteLine($"Generated: {DateTime.Now}");
                sw.WriteLine($"Generated by: Task Manager T4");
                sw.WriteLine($"Computer: {Environment.MachineName}");
                sw.WriteLine(new string('=', 50));

                float cpuUsage = GetCurrentCpuUsage();
                float memoryUsagePercent = GetCurrentMemoryUsage();
                long totalMemory = GetTotalSystemMemory();
                long usedMemory = (long)(memoryUsagePercent / 100.0 * totalMemory);

                sw.WriteLine("\n[Current System Load]");
                sw.WriteLine($"  CPU Usage: {cpuUsage:F2}%");
                sw.WriteLine($"  Memory Usage: {memoryUsagePercent:F2}%");
                sw.WriteLine($"  Used Memory: {FormatBytes(usedMemory)}");
                sw.WriteLine($"  Total Memory: {FormatBytes(totalMemory)}");
                sw.WriteLine($"  Free Memory: {FormatBytes(totalMemory - usedMemory)}");

                sw.WriteLine(new string('-', 40));

                sw.WriteLine("\n[CPU Information]");
                sw.WriteLine($"  Processor Count: {Environment.ProcessorCount}");
                sw.WriteLine($"  Logical Processors: {Environment.ProcessorCount}");
                sw.WriteLine($"  Guaranteed/Total: 1 / {Environment.ProcessorCount}");

                sw.WriteLine(new string('-', 40));

                sw.WriteLine("\n[Memory Information]");
                if (OperatingSystem.IsWindows())
                {
                    MEMORYSTATUSEX memStatus = new() { dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MEMORYSTATUSEX>() };
                    if (GlobalMemoryStatusEx(ref memStatus))
                    {
                        sw.WriteLine($"  Physical Memory: {FormatBytes((long)memStatus.ullTotalPhys)}");
                        sw.WriteLine($"  Available Memory: {FormatBytes((long)memStatus.ullAvailPhys)}");
                        sw.WriteLine($"  Total Page File: {FormatBytes((long)memStatus.ullTotalPageFile)}");
                        sw.WriteLine($"  Available Page File: {FormatBytes((long)memStatus.ullAvailPageFile)}");
                        sw.WriteLine($"  Virtual Memory: {FormatBytes((long)memStatus.ullTotalVirtual)}");
                        sw.WriteLine($"  Available Virtual: {FormatBytes((long)memStatus.ullAvailVirtual)}");
                        sw.WriteLine($"  Memory Load: {memStatus.dwMemoryLoad}%");
                    }
                }

                sw.WriteLine(new string('=', 50));
                sw.WriteLine("Report saved successfully!");
            }

            AnsiConsole.MarkupLine($"[{GraphicSettings.AccentColor}]✓[/] System load report saved: {reportFile}");

            // Спрашиваем, открыть ли папку
            if (AnsiConsole.Confirm("Open containing folder?"))
            {
                Process.Start("explorer.exe", folderPath);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error saving report: {ex.Message}[/]");
        }
    }

    private static float GetCurrentCpuUsage()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                using PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
                cpuCounter.NextValue(); 
                Thread.Sleep(100);
                return cpuCounter.NextValue();
            }
        }
        catch { }
        return new Random().Next(0, 100);
    }

    private static float GetCurrentMemoryUsage()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                MEMORYSTATUSEX memStatus = new() { dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MEMORYSTATUSEX>() };
                if (GlobalMemoryStatusEx(ref memStatus))
                {
                    return memStatus.dwMemoryLoad;
                }
            }
        }
        catch { }

        return new Random().Next(10, 80);
    }

    private static string FormatBytes(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        int counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:n1} {suffixes[counter]}";
    }

    private static string GetCpuGuaranteedMaxInfo()
    {
        if (OperatingSystem.IsWindows())
        {
            int maxCpu = Environment.ProcessorCount;
            int guaranteedCpu = 1;

            return $"{guaranteedCpu} / {maxCpu}";
        }
        else
        {
            int maxCpu = Environment.ProcessorCount;
            int guaranteedCpu = 1;

            return $"{guaranteedCpu} / {maxCpu}";
        }
    }
    private static long GetTotalSystemMemory()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                return GetWindowsTotalMemory();
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                return 16L * 1024 * 1024 * 1024;
            }
            else
            {
                return 8L * 1024 * 1024 * 1024;
            }
        }
        catch
        {
            return 8L * 1024 * 1024 * 1024;
        }
    }
    private static long GetWindowsTotalMemory()
    {
        MEMORYSTATUSEX memStatus = new()
        {
            dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MEMORYSTATUSEX>()
        };

        if (GlobalMemoryStatusEx(ref memStatus))
        {
            return (long)memStatus.ullTotalPhys;
        }

        return 8L * 1024 * 1024 * 1024;
    }
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }
}