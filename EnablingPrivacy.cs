using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using Spectre.Console;
using Task_Manager_T4;
using System.Threading;

class EnablingPrivacy : Other
{
    
    private class RegistrySetting
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public RegistryValueKind ValueKind { get; set; }
        public string Description { get; set; }
        public RegistryHive Hive { get; set; }
    }

    
    private static readonly List<RegistrySetting> PrivacySettings =
    [
        
        new RegistrySetting
        {
            Name = "Отключить рекламный идентификатор",
            Path = @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает рекламный идентификатор для персонализации рекламы",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить рекламу Bluetooth",
            Path = @"SOFTWARE\Microsoft\PolicyManager\current\device\Bluetooth",
            Key = "AllowAdvertising",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает рекламные объявления через Bluetooth",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить синхронизацию специальных возможностей",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Accessibility",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию настроек специальных возможностей",
            Hive = RegistryHive.CurrentUser
        },
        new RegistrySetting
        {
            Name = "Отключить синхронизацию браузера",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\BrowserSettings",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию настроек браузера",
            Hive = RegistryHive.CurrentUser
        },
        new RegistrySetting
        {
            Name = "Отключить синхронизацию учетных данных",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Credentials",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию учетных данных",
            Hive = RegistryHive.CurrentUser
        },
        new RegistrySetting
        {
            Name = "Отключить синхронизацию языка",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Language",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию языковых настроек",
            Hive = RegistryHive.CurrentUser
        },
        new RegistrySetting
        {
            Name = "Отключить синхронизацию персонализации",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Personalization",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию тем и персонализации",
            Hive = RegistryHive.CurrentUser
        },
        new RegistrySetting
        {
            Name = "Отключить синхронизацию Windows",
            Path = @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Windows",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает синхронизацию параметров Windows",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить Diagtrack-Listener",
            Path = @"SYSTEM\CurrentControlSet\Control\WMI\Autologger\Diagtrack-Listener",
            Key = "Start",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор диагностических данных",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Настройки вложений",
            Path = @"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments",
            Key = "SaveZoneInformation",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сохранение информации о зоне для вложений",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить программу улучшения качества ПО",
            Path = @"SOFTWARE\Microsoft\PolicyManager\current\device\System",
            Key = "AllowExperimentation",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает режим подопытного кролика",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить фоновое обновление речи",
            Path = @"SOFTWARE\Policies\Microsoft\Speech",
            Key = "AllowSpeechModelUpdate",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает автоматическое обновление моделей речи",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить мониторинг обращений",
            Path = @"Software\Microsoft\Siuf\Rules",
            Key = "NumberOfSIUFInPeriod",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор данных об обращениях в поддержку",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить геолокацию",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors",
            Key = "DisableLocation",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает определение местоположения",
            Hive = RegistryHive.LocalMachine
        },
        new RegistrySetting
        {
            Name = "Отключить датчики",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors",
            Key = "DisableSensors",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает работу датчиков",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить телеметрию рукописного ввода",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\TabletPC",
            Key = "PreventHandwritingDataSharing",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор данных рукописного ввода",
            Hive = RegistryHive.LocalMachine
        },
        new RegistrySetting
        {
            Name = "Отключить отчеты об ошибках рукописного ввода",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\HandwritingErrorReports",
            Key = "PreventHandwritingErrorReports",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает отправку отчетов об ошибках рукописного ввода",
            Hive = RegistryHive.LocalMachine
        },
        new RegistrySetting
        {
            Name = "Отключить телеметрию ввода",
            Path = @"Software\Microsoft\Input\TIPC",
            Key = "Enabled",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор телеметрии ввода с клавиатуры",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Уровень телеметрии",
            Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection",
            Key = "AllowTelemetry",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Устанавливает минимальный уровень сбора телеметрии (0)",
            Hive = RegistryHive.LocalMachine
        },
        new RegistrySetting
        {
            Name = "Отключить совместимость приложений",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\AppCompat",
            Key = "DisableInventory",
            Value = 1,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор данных о совместимости приложений",
            Hive = RegistryHive.LocalMachine
        },
        new RegistrySetting
        {
            Name = "Отключить сбор телеметрии",
            Path = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
            Key = "AllowTelemetry",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор телеметрии через политики",
            Hive = RegistryHive.LocalMachine
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить отслеживание телеметрии в проводнике",
            Path = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
            Key = "EnableTelemetry",
            Value = 0,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает сбор телеметрии в проводнике Windows",
            Hive = RegistryHive.CurrentUser
        },
        
        
        new RegistrySetting
        {
            Name = "Отключить телеметрию NVIDIA",
            Path = @"SYSTEM\CurrentControlSet\Services\NvTelemetryContainer",
            Key = "Start",
            Value = 4,
            ValueKind = RegistryValueKind.DWord,
            Description = "Отключает службу телеметрии NVIDIA",
            Hive = RegistryHive.LocalMachine
        }
    ];

    
    private static readonly Dictionary<string, string> ServicesToDisable = new()
    {
        { "DiagTrack", "Служба диагностического слежения" },
        { "dmwappushservice", "Служба маршрутизации push-сообщений WAP" },
        { "diagnosticshub.standardcollector.service", "Стандартная служба сбора диагностики" },
        { "CDPUserSvc", "Служба платформы подключенных устройств" }
    };

    
    public static void ShowPrivacyMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Windows Privacy & Telemetry[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{GraphicSettings.SecondaryColor}]Select action:[/]")
                    .PageSize(GraphicSettings.PageSize)
                    .AddChoices([
                        "Apply All Privacy Settings",
                        "Select Privacy Settings to Apply",
                        "View Current Privacy Status",
                        "Disable Telemetry Services",
                        "Restore Default Settings",
                        "Export Current Settings",
                        "Delete Telemetry Services (Advanced)",
                        "Back"
                    ]));

            switch (choice)
            {
                case "Apply All Privacy Settings":
                    ApplyAllPrivacySettings();
                    break;
                case "Select Privacy Settings to Apply":
                    SelectAndApplySettings();
                    break;
                case "View Current Privacy Status":
                    ViewPrivacyStatus();
                    break;
                case "Disable Telemetry Services":
                    DisableTelemetryServices();
                    break;
                case "Restore Default Settings":
                    RestoreDefaultSettings();
                    break;
                case "Export Current Settings":
                    ExportCurrentSettings();
                    break;
                case "Delete Telemetry Services (Advanced)":
                    DeleteTelemetryServices();
                    break;
                case "Back":
                    return;
            }
        }
    }

    
    private static void ApplyAllPrivacySettings()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Applying All Privacy Settings[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        if (!IsRunningAsAdmin())
        {
            AnsiConsole.MarkupLine($"[red]Administrator rights required![/]");
            Console.ReadKey();
            return;
        }

        var results = new Dictionary<string, bool>();

        AnsiConsole.Status()
            .Start("Applying privacy settings...", ctx =>
            {
                int total = PrivacySettings.Count;
                int current = 0;

                foreach (var setting in PrivacySettings)
                {
                    current++;
                    ctx.Status($"[{GraphicSettings.NeutralColor}][{current}/{total}] {setting.Name}...[/]");

                    bool success = SetRegistryValue(setting);
                    results[setting.Name] = success;

                    Thread.Sleep(50);
                }
            });

        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(GraphicSettings.GetColor(GraphicSettings.AccentColor))
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Setting[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Status[/]").Centered());

        foreach (var result in results)
        {
            string status;
            if (result.Value)
            {
                status = $"[green]Applied[/]";
            }
            else
            {
                status = $"[red]Failed[/]";
            }

            table.AddRow(
                $"[{GraphicSettings.SecondaryColor}]{result.Key}[/]",
                status
            );
        }

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void SelectAndApplySettings()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Select Privacy Settings[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        if (!IsRunningAsAdmin())
        {
            AnsiConsole.MarkupLine($"[red]Administrator rights required![/]");
            Console.ReadKey();
            return;
        }

        var selected = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]Select settings to apply:[/]")
                .NotRequired()
                .PageSize(15)
                .AddChoices(PrivacySettings.Select(s => s.Name)));

        if (selected.Count == 0) return;

        var settingsToApply = PrivacySettings.Where(s => selected.Contains(s.Name)).ToList();

        AnsiConsole.Status()
            .Start("Applying selected settings...", ctx =>
            {
                foreach (var setting in settingsToApply)
                {
                    ctx.Status($"[{GraphicSettings.NeutralColor}]Applying: {setting.Name}...[/]");

                    if (SetRegistryValue(setting))
                    {
                        AnsiConsole.MarkupLine($"[green]✓[/] [{GraphicSettings.SecondaryColor}]{setting.Name}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]✗[/] [{GraphicSettings.SecondaryColor}]{setting.Name}[/]");
                    }

                    Thread.Sleep(50);
                }
            });

        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void ViewPrivacyStatus()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Current Privacy Settings Status[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(GraphicSettings.GetColor(GraphicSettings.AccentColor))
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Setting[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Current Value[/]").Centered())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Recommended[/]").Centered())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Status[/]").Centered());

        foreach (var setting in PrivacySettings)
        {
            object currentValue = GetRegistryValue(setting);
            string currentStr = currentValue?.ToString() ?? "Not set";
            string recommendedStr = setting.Value.ToString();

            string status;
            string statusColor;

            if (currentValue == null)
            {
                status = "⚠ Not configured";
                statusColor = "yellow";
            }
            else if (currentValue.ToString() == setting.Value.ToString())
            {
                status = "✓ Optimized";
                statusColor = "green";
            }
            else
            {
                status = "✗ Not optimized";
                statusColor = "red";
            }

            table.AddRow(
                $"[{GraphicSettings.SecondaryColor}]{setting.Name}[/]",
                currentStr,
                recommendedStr,
                $"[{statusColor}]{status}[/]"
            );
        }

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void DisableTelemetryServices()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Disabling Telemetry Services[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        if (!IsRunningAsAdmin())
        {
            AnsiConsole.MarkupLine($"[red]Administrator rights required![/]");
            Console.ReadKey();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(GraphicSettings.GetColor(GraphicSettings.AccentColor))
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Service[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Status[/]").Centered());

        foreach (var service in ServicesToDisable)
        {
            try
            {
                
                ProcessStartInfo stopPsi = new()
                {
                    FileName = "net.exe",
                    Arguments = $"stop \"{service.Key}\" /y",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                Process.Start(stopPsi)?.WaitForExit();

                
                ProcessStartInfo configPsi = new()
                {
                    FileName = "sc.exe",
                    Arguments = $"config \"{service.Key}\" start= disabled",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                var process = Process.Start(configPsi);
                process?.WaitForExit();

                if (process?.ExitCode == 0)
                {
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{service.Value}[/]",
                        $"[green]✓ Disabled[/]"
                    );
                }
                else
                {
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{service.Value}[/]",
                        $"[red]✗ Failed[/]"
                    );
                }
            }
            catch (Exception ex)
            {
                table.AddRow(
                    $"[{GraphicSettings.SecondaryColor}]{service.Value}[/]",
                    $"[red]✗ {ex.Message}[/]"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void DeleteTelemetryServices()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]⚠ DELETE TELEMETRY SERVICES ⚠[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        AnsiConsole.MarkupLine($"[red]WARNING: This will completely remove telemetry services from Windows![/]");
        AnsiConsole.MarkupLine($"[yellow]This action cannot be undone without reinstalling Windows![/]\n");

        if (!AnsiConsole.Confirm("[red]Are you absolutely sure you want to delete these services?[/]", false))
        {
            return;
        }

        if (!AnsiConsole.Confirm("[red]FINAL WARNING: This may affect Windows Update and system stability! Continue?[/]", false))
        {
            return;
        }

        if (!IsRunningAsAdmin())
        {
            AnsiConsole.MarkupLine($"[red]❌ Administrator rights required![/]");
            Console.ReadKey();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(GraphicSettings.GetColor(GraphicSettings.AccentColor))
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Service[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Status[/]").Centered());

        string[] servicesToDelete = { "DiagTrack", "dmwappushservice" };

        foreach (var service in servicesToDelete)
        {
            try
            {
                
                ProcessStartInfo stopPsi = new()
                {
                    FileName = "net.exe",
                    Arguments = $"stop \"{service}\" /y",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                Process.Start(stopPsi)?.WaitForExit();

                
                ProcessStartInfo deletePsi = new()
                {
                    FileName = "sc.exe",
                    Arguments = $"delete \"{service}\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                var process = Process.Start(deletePsi);
                process?.WaitForExit();

                if (process?.ExitCode == 0)
                {
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{service}[/]",
                        $"[red]✓ Deleted[/]"
                    );
                }
                else
                {
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{service}[/]",
                        $"[red]✗ Failed[/]"
                    );
                }
            }
            catch (Exception ex)
            {
                table.AddRow(
                    $"[{GraphicSettings.SecondaryColor}]{service}[/]",
                    $"[red]✗ {ex.Message}[/]"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void RestoreDefaultSettings()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]Restore Default Settings[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

        if (!AnsiConsole.Confirm($"[{GraphicSettings.AccentColor}]Restore Windows privacy settings to default?[/]", false))
        {
            return;
        }

        
        AnsiConsole.Status()
            .Start("Restoring default settings...", ctx =>
            {
                foreach (var setting in PrivacySettings)
                {
                    try
                    {
                        using var key = RegistryKey.OpenBaseKey(setting.Hive, RegistryView.Registry64);
                        using var subKey = key.OpenSubKey(setting.Path, true);
                        if (subKey != null && subKey.GetValue(setting.Key) != null)
                        {
                            subKey.DeleteValue(setting.Key);
                        }
                    }
                    catch { }
                }
            });

        AnsiConsole.MarkupLine($"[{GraphicSettings.AccentColor}]✓ Default settings restored[/]");
        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static void ExportCurrentSettings()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "SystemReport");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, $"privacy_settings_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            using (StreamWriter sw = new(filePath))
            {
                sw.WriteLine("=== WINDOWS PRIVACY SETTINGS EXPORT ===");
                sw.WriteLine($"Date: {DateTime.Now}");
                sw.WriteLine($"Computer: {Environment.MachineName}");
                sw.WriteLine();

                foreach (var setting in PrivacySettings)
                {
                    object currentValue = GetRegistryValue(setting);

                    sw.WriteLine($"[{setting.Name}]");
                    sw.WriteLine($"  Path: {setting.Hive}\\{setting.Path}");
                    sw.WriteLine($"  Key: {setting.Key}");
                    sw.WriteLine($"  Current Value: {currentValue ?? "Not set"}");
                    sw.WriteLine($"  Recommended: {setting.Value}");
                    sw.WriteLine($"  Description: {setting.Description}");
                    sw.WriteLine();
                }
            }

            AnsiConsole.MarkupLine($"[{GraphicSettings.AccentColor}]Settings exported to: {filePath}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Export failed: {ex.Message}[/]");
        }

        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }

    
    private static bool SetRegistryValue(RegistrySetting setting)
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(setting.Hive, RegistryView.Registry64);
            using var subKey = key.CreateSubKey(setting.Path);
            if (subKey != null)
            {
                subKey.SetValue(setting.Key, setting.Value, setting.ValueKind);
                return true;
            }
        }
        catch (UnauthorizedAccessException)
        {
            
        }
        catch (Exception)
        {
            
        }
        return false;
    }

    
    private static object GetRegistryValue(RegistrySetting setting)
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(setting.Hive, RegistryView.Registry64);
            using var subKey = key.OpenSubKey(setting.Path);
            return subKey?.GetValue(setting.Key);
        }
        catch
        {
            return null;
        }
    }

    
    private static new bool IsRunningAsAdmin()
    {
        try
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    
    public static void CleanSpecificKeys()
    {
        
        var additionalSettings = new List<RegistrySetting>
        {
            new() {
                Name = "Очистка DiagTrack",
                Path = @"SYSTEM\CurrentControlSet\Services\DiagTrack",
                Key = "Start",
                Value = 4,
                ValueKind = RegistryValueKind.DWord,
                Hive = RegistryHive.LocalMachine
            },
            new() {
                Name = "Очистка dmwappushservice",
                Path = @"SYSTEM\CurrentControlSet\Services\dmwappushservice",
                Key = "Start",
                Value = 4,
                ValueKind = RegistryValueKind.DWord,
                Hive = RegistryHive.LocalMachine
            }
        };

        foreach (var setting in additionalSettings)
        {
            SetRegistryValue(setting);
        }
    }
}