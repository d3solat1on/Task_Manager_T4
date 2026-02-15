using System;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Linq;
using Spectre.Console;

namespace Task_Manager_T4;

public class GetInfoPc
{
    static bool IsUserAdministrator()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();

            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);

        }
        catch { return false; }
    }

    public static void Main_Information_Collection()
    {
        Console.Clear();

        try
        {
            AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]System Information report[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());
            AnsiConsole.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            var generalInfo = new Panel(
                $"[{GraphicSettings.AccentColor}]General Information[/]\n\n" +
                $"Computer: [{GraphicSettings.SecondaryColor}]{Environment.MachineName}[/]\n" +
                $"User: [{GraphicSettings.SecondaryColor}]{Environment.UserName}[/]\n" +
                $"OS: [{GraphicSettings.SecondaryColor}]{Environment.OSVersion}[/]\n" +
                $"Processors: [{GraphicSettings.SecondaryColor}]{Environment.ProcessorCount}[/]\n" +
                $"Admin: [{GraphicSettings.SecondaryColor}]{(IsUserAdministrator() ? "Yes" : "No")}[/]\n" +
                $"Uptime: [{GraphicSettings.SecondaryColor}]{TimeSpan.FromMilliseconds(Environment.TickCount):dd\\.hh\\:mm\\:ss}[/]")
                .BorderColor(GraphicSettings.GetThemeColor)
                .Padding(1, 1);

            string cpuInfo = "Not available";
            string gpuInfo = "Not available";

            try
            {
                cpuInfo = GetHardwareInfoSimple("Win32_Processor", "Name");
                gpuInfo = GetHardwareInfoSimple("Win32_VideoController", "Name");
            }
            catch { }

            var hardwareInfo = new Panel(
                $"[{GraphicSettings.AccentColor}]Hardware Information[/]\n\n" +
                $"CPU: [{GraphicSettings.SecondaryColor}]{cpuInfo}[/]\n" +
                $"GPU: [{GraphicSettings.SecondaryColor}]{gpuInfo}[/]\n" +
                $"64-bit OS: [{GraphicSettings.SecondaryColor}]{(Environment.Is64BitOperatingSystem ? "Yes" : "No")}[/]\n" +
                $".NET: [{GraphicSettings.SecondaryColor}]{Environment.Version}[/]")
                .BorderColor(GraphicSettings.GetThemeColor)
                .Padding(1, 1);

            grid.AddRow(generalInfo, hardwareInfo);
            AnsiConsole.Write(grid);

            AnsiConsole.WriteLine();
            ShowDriveInfoSimple();

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"\n[{GraphicSettings.SecondaryColor}]For a detailed report, it is recommended to run the application as administrator.[/]");
            if (AnsiConsole.Confirm($"[{GraphicSettings.AccentColor}]Create detailed report file?[/]", true))
            {
                InfoPC.CreateDetailedReport();
            }

            AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
            Console.ReadKey();
            Console.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    static string GetHardwareInfoSimple(string win32Class, string classProperty)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {classProperty} FROM {win32Class}");
            var result = new System.Text.StringBuilder();

            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                string value = obj[classProperty]?.ToString() ?? "";

                value = value.Replace("[", "").Replace("]", "");
                if (!string.IsNullOrEmpty(value))
                {
                    result.AppendLine(value.Trim());
                }
            }

            string info = result.ToString().Trim();
            return string.IsNullOrEmpty(info) ? "Not available" : info.Split('\n')[0];
        }
        catch
        {
            return "Not available";
        }
    }

    private static void ShowDriveInfoSimple()
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);

            var table = new Table()
                .Title($"[{GraphicSettings.AccentColor}]Storage Drives[/]")
                .Border(TableBorder.Simple)
                .BorderColor(GraphicSettings.GetThemeColor)
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Drive[/]")
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Label[/]")
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Type[/]")
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Total[/]")
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Free[/]")
                .AddColumn($"[{GraphicSettings.SecondaryColor}]Usage[/]");

            foreach (var drive in drives)
            {
                double totalGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                double freeGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0);
                double usedPercent = 100 - (drive.TotalFreeSpace * 100 / drive.TotalSize);

                string usageBar = GetSimpleUsageBar(usedPercent);

                table.AddRow(
                    $"[{GraphicSettings.AccentColor}]{drive.Name}[/]",
                    $"[{GraphicSettings.SecondaryColor}]{drive.VolumeLabel}[/]",
                    $"[{GraphicSettings.SecondaryColor}]{drive.DriveType}[/]",
                    $"[{GraphicSettings.SecondaryColor}]{totalGB:F1} GB[/]",
                    $"[{GraphicSettings.SecondaryColor}]{freeGB:F1} GB[/]",
                    $"[{GraphicSettings.SecondaryColor}]{usageBar} {usedPercent:F1}%[/]");
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading drive info: {ex.Message}[/]");
        }
    }

    private static string GetSimpleUsageBar(double percent)
    {
        int filled = (int)(percent / 10);
        return new string('█', filled) + new string('░', 10 - filled);
    }

    public static void ShowDriveInfo()
    {
        var drives = DriveInfo.GetDrives().Where(d => d.IsReady);

        var table = new Table()
            .Title($"[{GraphicSettings.AccentColor}]Storage Drives[/]")
            .Border(TableBorder.Rounded)
            .BorderColor(GraphicSettings.GetThemeColor)
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Drive[/]").Centered())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Label[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Type[/]").Centered())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Total[/]").RightAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Free[/]").RightAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Usage[/]").Centered());

        foreach (var drive in drives)
        {
            double freePercent = (double)drive.TotalFreeSpace / drive.TotalSize * 100;
            string usageBar = GetUsageBar(freePercent);
            string color = freePercent > 20 ? "green" : freePercent > 10 ? "yellow" : "red";

            table.AddRow(
                $"[{GraphicSettings.AccentColor}]{drive.Name}[/]",
                $"[{GraphicSettings.SecondaryColor}]{drive.VolumeLabel}[/]",
                $"[{GraphicSettings.NeutralColor}]{drive.DriveType}[/]",
                $"[{GraphicSettings.SecondaryColor}]{drive.TotalSize / 1_000_000_000:N0} GB[/]",
                $"[{color}]{drive.TotalFreeSpace / 1_000_000_000:N0} GB[/]",
                $"[{color}]{usageBar} {freePercent:N1}%[/]");
        }

        AnsiConsole.Write(table);
    }

    private static string GetUsageBar(double percent)
    {
        int filled = (int)(percent / 100 * 10);
        int empty = 10 - filled;
        return $"[{new string('█', filled)}{new string('░', empty)}]";
    }
}