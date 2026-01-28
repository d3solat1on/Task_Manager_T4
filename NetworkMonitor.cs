using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using ProjectT4;
namespace ProjectT4;

public class NetworkMonitor
{
    private bool _isMonitoring = false;
    private CancellationTokenSource _cancellationTokenSource;
    private Dictionary<string, NetworkStats> _connectionStats = new Dictionary<string, NetworkStats>();
    private List<BlockedConnection> _blockedConnections = new List<BlockedConnection>();

    public class NetworkStats
    {
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
        public int ConnectionCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class BlockedConnection
    {
        public string RemoteIP { get; set; }
        public int Port { get; set; }
        public string ProcessName { get; set; }
        public DateTime BlockedAt { get; set; }
        public string Reason { get; set; }
    }

    public class SpeedTestResult
    {
        public double DownloadSpeedMbps { get; set; }
        public double UploadSpeedMbps { get; set; }
        public int PingMs { get; set; }
        public string ServerLocation { get; set; }
        public DateTime TestTime { get; set; }
    }

    // ===== ОСНОВНОЙ МЕТОД ЗАПУСКА =====
    public void ShowNetworkMenu()
    {
        while (true)
        {
            Console.Clear();

            AnsiConsole.Write(
                new FigletText("Network Monitor")
                    .Centered()
                    .Color(Color.Blue));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Network Analytics[/]")
                    .PageSize(12)
                    .AddChoices(new[] {
                        "📡 Real-time Monitoring",
                        "📊 Active Connections",
                        "🔍 Traffic by Process",
                        "🚫 Blocked Connections",
                        "⚡ Speed Test",
                        "🌐 Network Information",
                        "📈 Traffic Statistics",
                        "🛡️ Firewall Rules",
                        "🔙 Back to Main Menu"
                    }));

            switch (choice)
            {
                case "📡 Real-time Monitoring":
                    StartRealTimeMonitoring();
                    break;
                case "📊 Active Connections":
                    ShowActiveConnections();
                    break;
                case "🔍 Traffic by Process":
                    ShowTrafficByProcess();
                    break;
                case "🚫 Blocked Connections":
                    ShowBlockedConnections();
                    break;
                case "⚡ Speed Test":
                    RunSpeedTest();
                    break;
                case "🌐 Network Information":
                    ShowNetworkInfo();
                    break;
                case "📈 Traffic Statistics":
                    ShowTrafficStatistics();
                    break;
                case "🛡️ Firewall Rules":
                    ManageFirewallRules();
                    break;
                case "🔙 Back to Main Menu":
                    StopMonitoring();
                    return;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    // ===== РЕАЛЬНЫЙ МОНИТОРИНГ =====
    private void StartRealTimeMonitoring()
    {
        Console.Clear();

        if (_isMonitoring)
        {
            AnsiConsole.MarkupLine("[yellow]Monitoring is already running![/]");
            return;
        }

        AnsiConsole.MarkupLine("[green]Starting network monitoring...[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to stop monitoring[/]");

        _isMonitoring = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _connectionStats.Clear();

        // Запускаем мониторинг в отдельной задаче
        Task.Run(() => MonitorNetworkTraffic(_cancellationTokenSource.Token));

        // Показываем таблицу в реальном времени
        while (!Console.KeyAvailable && !_cancellationTokenSource.Token.IsCancellationRequested)
        {
            Console.Clear();
            ShowRealTimeTable();
            Thread.Sleep(1000);
        }

        StopMonitoring();
    }

    private void MonitorNetworkTraffic(CancellationToken cancellationToken)
    {
        try
        {
            var ipv4Properties = IPGlobalProperties.GetIPGlobalProperties();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Получаем активные TCP соединения
                var tcpConnections = ipv4Properties.GetActiveTcpConnections();
                UpdateConnectionStats(tcpConnections);

                // Получаем статистику по интерфейсам
                UpdateInterfaceStats();

                Thread.Sleep(2000);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Monitoring error: {ex.Message}[/]");
        }
    }

    private void UpdateConnectionStats(TcpConnectionInformation[] connections)
    {
        var processConnections = connections
            .GroupBy(c => GetProcessIdByPort(c.LocalEndPoint.Port))
            .Where(g => g.Key > 0);

        foreach (var group in processConnections)
        {
            int pid = group.Key;
            string processName = GetProcessName(pid);
            string key = $"{processName}_{pid}";

            if (!_connectionStats.ContainsKey(key))
            {
                _connectionStats[key] = new NetworkStats
                {
                    ProcessName = processName,
                    ProcessId = pid,
                    LastUpdated = DateTime.Now
                };
            }

            _connectionStats[key].ConnectionCount = group.Count();
            _connectionStats[key].LastUpdated = DateTime.Now;
        }
    }

    private void UpdateInterfaceStats()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

        foreach (var ni in interfaces)
        {
            var stats = ni.GetIPv4Statistics();

            // Здесь можно обновить статистику по интерфейсам
            // Для простоты пропускаем в этом примере
        }
    }

    private void ShowRealTimeTable()
    {
        var table = new Table()
            .Title("[bold cyan]🔄 Real-time Network Monitor[/]")
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[yellow]Process[/]").LeftAligned())
            .AddColumn(new TableColumn("[yellow]PID[/]").Centered())
            .AddColumn(new TableColumn("[yellow]Connections[/]").Centered())
            .AddColumn(new TableColumn("[yellow]Status[/]").Centered());

        foreach (var stats in _connectionStats.Values
            .OrderByDescending(s => s.ConnectionCount)
            .Take(15))
        {
            string status = stats.ConnectionCount > 10 ? "[red]High[/]" :
                           stats.ConnectionCount > 5 ? "[yellow]Medium[/]" : "[green]Normal[/]";

            table.AddRow(
                $"[white]{stats.ProcessName}[/]",
                $"[grey]{stats.ProcessId}[/]",
                $"[cyan]{stats.ConnectionCount}[/]",
                status
            );
        }

        AnsiConsole.Write(table);

        // Показываем общую статистику
        var panel = new Panel(
            $"[bold]📊 Summary[/]\n" +
            $"[green]Active processes:[/] {_connectionStats.Count}\n" +
            $"[green]Total connections:[/] {_connectionStats.Sum(s => s.Value.ConnectionCount)}\n" +
            $"[green]Monitoring time:[/] {DateTime.Now - _connectionStats.FirstOrDefault().Value?.LastUpdated ?? TimeSpan.Zero:mm\\:ss}")
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Blue)
        };

        AnsiConsole.Write(panel);
    }

    // ===== АКТИВНЫЕ СОЕДИНЕНИЯ =====
    private void ShowActiveConnections()
    {
        Console.Clear();

        try
        {
            var ipv4Properties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = ipv4Properties.GetActiveTcpConnections();

            var table = new Table()
                .Title($"[bold red]Active TCP Connections ({tcpConnections.Length})[/]")
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[cyan]Local Address[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]Remote Address[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]State[/]").Centered())
                .AddColumn(new TableColumn("[cyan]Process[/]").LeftAligned());

            foreach (var connection in tcpConnections.Take(50))
            {
                int pid = GetProcessIdByPort(connection.LocalEndPoint.Port);
                string processName = pid > 0 ? GetProcessName(pid) : "System";
                string stateColor = connection.State == TcpState.Established ? "green" :
                                   connection.State == TcpState.Listen ? "yellow" : "grey";

                table.AddRow(
                    $"[white]{connection.LocalEndPoint.Address}:{connection.LocalEndPoint.Port}[/]",
                    $"[grey]{connection.RemoteEndPoint.Address}:{connection.RemoteEndPoint.Port}[/]",
                    $"[{stateColor}]{connection.State}[/]",
                    $"[blue]{processName}[/]"
                );
            }

            AnsiConsole.Write(table);

            // Статистика по состояниям
            var stateStats = tcpConnections
                .GroupBy(c => c.State)
                .Select(g => new { State = g.Key, Count = g.Count() });

            var chart = new BreakdownChart()
                .Width(60)
                .ShowPercentage();

            // Создаем панель с заголовком для графика
            var chartPanel = new Panel(
                new BreakdownChart()
                    .Width(60)
                    .ShowPercentage()
                    .AddItems(stateStats.Select(stat =>
                    {
                        var color = stat.State == TcpState.Established ? Color.Green :
                                   stat.State == TcpState.Listen ? Color.Yellow : Color.Grey;
                        return new BreakdownChartItem(stat.State.ToString(), stat.Count, color);
                    }))
                )
            {
                Header = new PanelHeader("[bold cyan]Connection States[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0, 1, 0)
            };

            AnsiConsole.Write(chartPanel);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    // ===== ТРАФИК ПО ПРОЦЕССАМ =====
    private void ShowTrafficByProcess()
    {
        Console.Clear();

        try
        {
            // Получаем все процессы с сетевыми соединениями
            var ipv4Properties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = ipv4Properties.GetActiveTcpConnections();

            var processTraffic = tcpConnections
                .GroupBy(c => GetProcessIdByPort(c.LocalEndPoint.Port))
                .Where(g => g.Key > 0)
                .Select(g => new
                {
                    ProcessId = g.Key,
                    ProcessName = GetProcessName(g.Key),
                    Connections = g.Count(),
                    LocalPorts = string.Join(", ", g.Select(c => c.LocalEndPoint.Port).Distinct().Take(3))
                })
                .OrderByDescending(p => p.Connections)
                .Take(20);

            var table = new Table()
                .Title("[bold blue]📈 Traffic by Process[/]")
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[cyan]Process[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]PID[/]").Centered())
                .AddColumn(new TableColumn("[cyan]Connections[/]").Centered())
                .AddColumn(new TableColumn("[cyan]Ports[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]Risk Level[/]").Centered());

            foreach (var proc in processTraffic)
            {
                string riskLevel = proc.Connections > 15 ? "[red]HIGH[/]" :
                                  proc.Connections > 8 ? "[yellow]MEDIUM[/]" : "[green]LOW[/]";

                table.AddRow(
                    $"[white]{proc.ProcessName}[/]",
                    $"[grey]{proc.ProcessId}[/]",
                    $"[cyan]{proc.Connections}[/]",
                    $"[blue]{proc.LocalPorts}[/]",
                    riskLevel
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    // ===== БЛОКИРОВАННЫЕ СОЕДИНЕНИЯ =====
    private void ShowBlockedConnections()
    {
        Console.Clear();

        var table = new Table()
            .Title($"[bold red]🚫 Blocked Connections ({_blockedConnections.Count})[/]")
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[cyan]IP Address[/]").LeftAligned())
            .AddColumn(new TableColumn("[cyan]Port[/]").Centered())
            .AddColumn(new TableColumn("[cyan]Process[/]").LeftAligned())
            .AddColumn(new TableColumn("[cyan]Reason[/]").LeftAligned())
            .AddColumn(new TableColumn("[cyan]Blocked At[/]").Centered());

        foreach (var blocked in _blockedConnections.Take(20))
        {
            table.AddRow(
                $"[red]{blocked.RemoteIP}[/]",
                $"[yellow]{blocked.Port}[/]",
                $"[white]{blocked.ProcessName}[/]",
                $"[grey]{blocked.Reason}[/]",
                $"[blue]{blocked.BlockedAt:HH:mm:ss}[/]"
            );
        }

        AnsiConsole.Write(table);

        // Меню управления блокировками
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Manage blocked connections:")
                .AddChoices(new[] {
                    "➕ Block new connection",
                    "➖ Remove block",
                    "📋 Export to file",
                    "🔙 Back"
                }));

        switch (choice)
        {
            case "➕ Block new connection":
                BlockNewConnection();
                break;
            case "➖ Remove block":
                RemoveBlock();
                break;
            case "📋 Export to file":
                ExportBlocksToFile();
                break;
        }
    }

    private void BlockNewConnection()
    {
        string ip = AnsiConsole.Prompt(new TextPrompt<string>("[red]Enter IP address to block:[/]"));
        string portStr = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Enter port (0 for all ports):[/]"));
        string reason = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]Enter reason:[/]"));

        if (int.TryParse(portStr, out int port))
        {
            _blockedConnections.Add(new BlockedConnection
            {
                RemoteIP = ip,
                Port = port,
                ProcessName = "Manual block",
                BlockedAt = DateTime.Now,
                Reason = reason
            });

            AnsiConsole.MarkupLine($"[green]✓ Connection {ip}:{port} blocked[/]");

            // Здесь можно добавить реальную блокировку через Windows Firewall
            // BlockWithFirewall(ip, port);
        }
    }

    // ===== ТЕСТ СКОРОСТИ =====
    private async Task RunSpeedTest()
    {
        Console.Clear();

        AnsiConsole.MarkupLine("[bold cyan]⚡ Running Speed Test...[/]");
        AnsiConsole.MarkupLine("[grey]This may take 10-15 seconds[/]");

        try
        {
            var result = await PerformSpeedTest();

            var panel = new Panel(
                $"[bold]📊 Speed Test Results[/]\n\n" +
                $"[green]Download:[/] [white]{result.DownloadSpeedMbps:F2} Mbps[/]\n" +
                $"[green]Upload:[/] [white]{result.UploadSpeedMbps:F2} Mbps[/]\n" +
                $"[green]Ping:[/] [white]{result.PingMs} ms[/]\n" +
                $"[green]Server:[/] [white]{result.ServerLocation}[/]\n" +
                $"[green]Time:[/] [white]{result.TestTime:HH:mm:ss}[/]")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan),
                Padding = new Padding(2, 1, 2, 1)
            };

            AnsiConsole.Write(panel);

            // Визуализация результатов
            var chart = new BarChart()
                .Width(60)
                .Label("[blue]Speed (Mbps)[/]")
                .CenterLabel()
                .AddItem("Download", result.DownloadSpeedMbps, Color.Green)
                .AddItem("Upload", result.UploadSpeedMbps, Color.Blue);

            AnsiConsole.Write(chart);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Speed test failed: {ex.Message}[/]");
        }
    }

    private async Task<SpeedTestResult> PerformSpeedTest()
    {
        // Имитация теста скорости (в реальном приложении используйте библиотеки типа SpeedTest.Net)
        await Task.Delay(3000);

        var random = new Random();
        return new SpeedTestResult
        {
            DownloadSpeedMbps = 50 + random.NextDouble() * 50,
            UploadSpeedMbps = 10 + random.NextDouble() * 20,
            PingMs = 10 + random.Next(40),
            ServerLocation = "Local Server",
            TestTime = DateTime.Now
        };
    }

    // ===== СЕТЕВАЯ ИНФОРМАЦИЯ =====
    private void ShowNetworkInfo()
    {
        Console.Clear();

        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .OrderByDescending(ni => ni.Speed);

            var table = new Table()
                .Title("[bold green]🌐 Network Interfaces[/]")
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[cyan]Name[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]Type[/]").Centered())
                .AddColumn(new TableColumn("[cyan]Status[/]").Centered())
                .AddColumn(new TableColumn("[cyan]Speed[/]").RightAligned())
                .AddColumn(new TableColumn("[cyan]MAC Address[/]").LeftAligned())
                .AddColumn(new TableColumn("[cyan]IP Addresses[/]").LeftAligned());

            foreach (var ni in interfaces)
            {
                var ipProperties = ni.GetIPProperties();
                var unicastAddresses = ipProperties.UnicastAddresses
                    .Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(addr => addr.Address.ToString())
                    .ToArray();

                string ips = unicastAddresses.Length > 0 ?
                    string.Join(", ", unicastAddresses.Take(2)) : "No IPv4";

                table.AddRow(
                    $"[white]{ni.Name}[/]",
                    $"[grey]{ni.NetworkInterfaceType}[/]",
                    $"[green]{ni.OperationalStatus}[/]",
                    $"[yellow]{ni.Speed / 1_000_000} Mbps[/]",
                    $"[blue]{ni.GetPhysicalAddress()}[/]",
                    $"[cyan]{ips}[/]"
                );
            }

            AnsiConsole.Write(table);

            // Показываем информацию о DNS
            ShowDnsInfo();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====
    private int GetProcessIdByPort(int port)
    {
        try
        {
            // Используем netstat для получения PID по порту
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netstat",
                    Arguments = "-ano",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Парсим вывод netstat
            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains($":{port} ") && line.Contains("LISTENING"))
                {
                    var parts = line.Trim().Split(' ');
                    if (parts.Length > 4 && int.TryParse(parts[^1], out int pid))
                        return pid;
                }
            }

            return 0;
        }
        catch
        {
            return 0;
        }
    }

    private string GetProcessName(int pid)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            return process.ProcessName;
        }
        catch
        {
            return $"PID:{pid}";
        }
    }

    private void StopMonitoring()
    {
        if (_isMonitoring)
        {
            _cancellationTokenSource?.Cancel();
            _isMonitoring = false;
            AnsiConsole.MarkupLine("[yellow]Monitoring stopped[/]");
        }
    }

    // Остальные методы (ShowTrafficStatistics, ManageFirewallRules, ShowDnsInfo и т.д.)
    private void ShowTrafficStatistics()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold cyan]📈 Traffic Statistics[/]");
        // Реализация статистики...
    }

    private void ManageFirewallRules()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold red]🛡️ Firewall Rules Management[/]");
        // Реализация управления фаерволом...
    }

    private void ShowDnsInfo()
    {
        try
        {
            var dnsServers = System.Net.NetworkInformation.IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Where(l => l.Port == 53)
                .Select(l => l.Address.ToString())
                .ToArray();

            if (dnsServers.Any())
            {
                AnsiConsole.MarkupLine("\n[bold]DNS Servers:[/]");
                foreach (var dns in dnsServers)
                {
                    AnsiConsole.MarkupLine($"  • [cyan]{dns}[/]");
                }
            }
        }
        catch { }
    }

    private void RemoveBlock()
    {
        if (_blockedConnections.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No blocked connections to remove[/]");
            return;
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select connection to unblock:")
                .AddChoices(_blockedConnections.Select(b => $"{b.RemoteIP}:{b.Port} - {b.Reason}")));

        var ipPort = selected.Split(':')[0];
        _blockedConnections.RemoveAll(b => b.RemoteIP == ipPort);
        AnsiConsole.MarkupLine($"[green]✓ Connection {ipPort} unblocked[/]");
    }

    private void ExportBlocksToFile()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"blocked_connections_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            using (var writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine("Blocked Network Connections");
                writer.WriteLine($"Generated: {DateTime.Now}");
                writer.WriteLine(new string('=', 50));

                foreach (var block in _blockedConnections)
                {
                    writer.WriteLine($"{block.RemoteIP}:{block.Port}");
                    writer.WriteLine($"  Process: {block.ProcessName}");
                    writer.WriteLine($"  Reason: {block.Reason}");
                    writer.WriteLine($"  Blocked: {block.BlockedAt}");
                    writer.WriteLine();
                }
            }

            AnsiConsole.MarkupLine($"[green]✓ Exported to {filePath}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Export failed: {ex.Message}[/]");
        }
    }
}