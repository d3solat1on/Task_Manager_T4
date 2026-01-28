using System;
using System.Threading;
using System.Diagnostics;
using Spectre.Console;
using ProjectT4;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = "Task Manager T4";

        AnsiConsole.Write(
            new FigletText("System Monitor")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.Write(
            new FigletText("Task Manager")
                .Centered()
                .Color(Color.Yellow));

        // Панель с информацией
        var panel = new Panel("[yellow]v1.0 • Created with Spectre.Console[/]")
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Cyan1),
            Padding = new Padding(1, 1, 1, 1)
        };

        AnsiConsole.Write(panel);


        var table = new Table()
            .Centered()
            .BorderColor(Color.Green)
            .Border(TableBorder.Rounded)
            .Title("[bold yellow]System Information[/]")
            .AddColumn(new TableColumn("[cyan]Component[/]").Centered())
            .AddColumn(new TableColumn("[cyan]Status[/]").Centered())
            .AddRow("[green]OS[/]", $"[white]{Environment.OSVersion}[/]")
            .AddRow("[green]User[/]", $"[white]{Environment.UserName}[/]")
            .AddRow("[green]Machine[/]", $"[white]{Environment.MachineName}[/]")
            .AddRow("[green]Processors[/]", $"[white]{Environment.ProcessorCount}[/]");

        AnsiConsole.Write(table);

        AnsiConsole.Write(new Rule("[yellow]Press any key to continue[/]").RuleStyle("yellow").Centered());

        Console.ReadKey();
        Console.Clear();
        Function_list();
    }

    public static void Function_list()
    {
        while (true)
        {
            Console.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Select an option:[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {
                    "📊 Process Management",
                    "💻 System Information",
                    "📸 Screenshot Tool",
                    "🚀 Program Launcher",
                    "🖥️ Show System Load",
                    "ShowNetworkMenu",
                    "⚡ Startup Manager",
                    "🌡️ Check Temperature",
                    "🔧 Service Manager", 
                    "🎨 OpenMe",
                    "❌ Exit"
                    }));

            switch (choice)
            {
                case "📊 Process Management":
                    Process_management.GetProcces();
                    break;
                case "💻 System Information":
                    GetInfoPc.Main_Information_Collection();
                    break;
                case "📸 Screenshot Tool":
                    GetInfoPc.TakeScreenshotMenu();
                    break;
                case "🚀 Program Launcher":
                    OpenProgram.OpenPrograms();
                    break;
                case "🖥️ Show System Load":
                    ShowSystemLoad();
                    break;
                case "ShowNetworkMenu":
                    NetworkMonitor networkMonitor = new();
                    networkMonitor.ShowNetworkMenu();
                    break;
                case "⚡ Startup Manager":
                    try
                    {
                        StartUpManager startupManager = new();
                        startupManager.ShowStartupManagerUI();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.ReadKey();
                    }
                    break;
                case "🌡️ Check Temperature":
                    AdvancedTemperatureMonitor.ShowAllTemperatures();
                    break;
                case "🔧 Service Manager": 
                    ServiceManagerUI.ShowServicesMenu();
                    break; 
                case "🎨 OpenMe":
                    Rain.ShowReadMeWithRain();
                    break;
                case "❌ Exit":
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private static void ShowSystemLoad()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold cyan]System Load Monitoring[/]");
        AnsiConsole.MarkupLine("[grey]Updates every two seconds. Press any key to exit.[/]");
        AnsiConsole.WriteLine();

        var table = new Table();
        table.Border = TableBorder.Rounded;

        table.AddColumn(new TableColumn("[bold]Time[/]").Centered());
        table.AddColumn(new TableColumn("[bold]CPU %[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Memory %[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Memory (bytes)[/]").Centered());
        table.AddColumn(new TableColumn("[bold]GTD / Max Memory (bytes)[/]").Centered());
        table.AddColumn(new TableColumn("[bold]GTD / Max CPU (units)[/]").Centered());

        // Инициализация счетчиков производительности (работает только на Windows)
        PerformanceCounter performanceCounterCpu = null;
        PerformanceCounter performanceCounterMemory = null;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                performanceCounterCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                performanceCounterMemory = new PerformanceCounter("Memory", "% Committed Bytes In Use");

                // Чтобы получить актуальные значения при первом вызове
                performanceCounterCpu.NextValue();
                performanceCounterMemory.NextValue();
                Thread.Sleep(1000);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Performance counters are only available on Windows.[/]");
                AnsiConsole.MarkupLine("[yellow]Using simulated data for demonstration.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error initializing performance counters: {ex.Message}[/]");
            AnsiConsole.MarkupLine("[yellow]Using simulated data for demonstration.[/]");
        }

        Random rand = new Random();
        long totalSystemMemory = GetTotalSystemMemory();

        while (!Console.KeyAvailable)
        {
            float cpuUsage;
            float memoryUsagePercent;
            long usedMemoryBytes;

            if (performanceCounterCpu != null && performanceCounterMemory != null)
            {
                // Реальные данные (только на Windows)
#pragma warning disable CA1416 // Проверка совместимости платформы
                cpuUsage = performanceCounterCpu.NextValue();
#pragma warning restore CA1416 // Проверка совместимости платформы
#pragma warning disable CA1416 // Проверка совместимости платформы
                memoryUsagePercent = performanceCounterMemory.NextValue();
#pragma warning restore CA1416 // Проверка совместимости платформы
                usedMemoryBytes = (long)(memoryUsagePercent / 100.0 * totalSystemMemory);
            }
            else
            {
                // Симулированные данные для других платформ или при ошибке
                cpuUsage = rand.Next(0, 100);
                memoryUsagePercent = rand.Next(10, 80);
                usedMemoryBytes = (long)(memoryUsagePercent / 100.0 * totalSystemMemory);
            }

            // Форматируем время
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            // Форматируем значения для таблицы
            string cpuFormatted = $"{cpuUsage:F2} %";
            string memoryPercentFormatted = $"{memoryUsagePercent:F2} %";
            string memoryBytesFormatted = $"{usedMemoryBytes:N0}";
            string memoryMaxFormatted = $"{usedMemoryBytes:N0} / {totalSystemMemory:N0}";
            string cpuMaxFormatted = GetCpuGuaranteedMaxInfo();

            // Добавляем строку в таблицу
            table.AddRow(
                currentTime,
                cpuFormatted,
                memoryPercentFormatted,
                memoryBytesFormatted,
                memoryMaxFormatted,
                cpuMaxFormatted
            );

            // Очищаем консоль и перерисовываем таблицу
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]System Load Monitoring[/]");

            if (performanceCounterCpu == null || performanceCounterMemory == null)
            {
                AnsiConsole.MarkupLine("[yellow]⚠ Using simulated data[/]");
            }

            AnsiConsole.MarkupLine("[grey]Updates every two seconds. Press any key to exit.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);

            Thread.Sleep(2000);
        }

        // Очищаем буфер ввода
        while (Console.KeyAvailable) Console.ReadKey(true);

        // Освобождаем ресурсы счетчиков
        performanceCounterCpu?.Dispose();
        performanceCounterMemory?.Dispose();
    }

    private static string GetCpuGuaranteedMaxInfo()
    {
        if (OperatingSystem.IsWindows())
        {
            // Для Windows можно получить информацию через WMI
            // Максимальное количество процессоров
            int maxCpu = Environment.ProcessorCount;

            // Гарантированный минимум (обычно 1, но может быть настроен)
            int guaranteedCpu = 1; // По умолчанию

            return $"{guaranteedCpu} / {maxCpu}";
        }
        else
        {
            // Для Linux можно прочитать из cgroups
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
                // Для Windows используем GlobalMemoryStatusEx через P/Invoke
                return GetWindowsTotalMemory();
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                // Для Linux/Mac можно парсить /proc/meminfo или использовать sysinfo
                // Здесь возвращаем фиктивное значение для демонстрации
                return 16L * 1024 * 1024 * 1024; // 16 GB
            }
            else
            {
                return 8L * 1024 * 1024 * 1024; // 8 GB по умолчанию
            }
        }
        catch
        {
            // Возвращаем значение по умолчанию в случае ошибки
            return 8L * 1024 * 1024 * 1024; // 8 GB
        }
    }

    // P/Invoke для получения информации о памяти в Windows
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

    private static long GetWindowsTotalMemory()
    {
        MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
        memStatus.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));

        if (GlobalMemoryStatusEx(ref memStatus))
        {
            return (long)memStatus.ullTotalPhys;
        }

        return 8L * 1024 * 1024 * 1024; // 8 GB по умолчанию
    }
}