using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Spectre.Console;
using Task_Manager_T4;

class OpenProgram
{
    public static void OpenPrograms()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Program Launcher[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

            var category = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{GraphicSettings.SecondaryColor}]Select Category:[/]")
                    .PageSize(GraphicSettings.PageSize)
                    .AddChoices([
                        "System Tools",
                        "Administrative Tools", 
                        "Control Panel",
                        "Custom File/Path",
                        "Back to Main Menu"
                    ]));
            
            switch (category)
            {
                case "System Tools":
                    OpenSystemTools();
                    break;
                case "Administrative Tools":
                    OpenAdministrativeTools();
                    break;
                case "Control Panel":
                    OpenControlPanel();
                    break;
                case "Custom File/Path":
                    OpenCustomFile();
                    break;
                case "Back to Main Menu":
                    Console.Clear();
                    return;
            }
        }
    }
    
    private static void OpenSystemTools()
    {
        Console.Clear();
        
        var tool = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]System Tools[/]")
                .PageSize(GraphicSettings.PageSize)
                .MoreChoicesText($"[{GraphicSettings.NeutralColor}](Move up/down to see more)[/]")
                .AddChoices([
                    "Command Prompt",
                    "PowerShell",
                    "Windows Terminal",
                    "Notepad",
                    "Paint",
                    "Calculator",
                    "File Explorer",
                    "WordPad",
                    "Voice Recorder",
                    "Camera",
                    "Media Player",
                    "Back"
                ]));
        
        try
        {
            switch (tool)
            {
                case "Command Prompt":
                    RunWithAnimation("cmd.exe", "Starting Command Prompt...");
                    break;
                case "PowerShell":
                    RunWithAnimation("powershell.exe", "Starting PowerShell...");
                    break;
                case "Windows Terminal":
                    RunWithAnimation("wt.exe", "Starting Windows Terminal...");
                    break;
                case "Notepad":
                    RunWithAnimation("notepad.exe", "Starting Notepad...");
                    break;
                case "Paint":
                    RunWithAnimation("mspaint.exe", "Starting Paint...");
                    break;
                case "Calculator":
                    RunWithAnimation("calc.exe", "Starting Calculator...");
                    break;
                case "File Explorer":
                    RunWithAnimation("explorer.exe", "Starting File Explorer...");
                    break;
                case "WordPad":
                    RunWithAnimation("write.exe", "Starting WordPad...");
                    break;
                case "Voice Recorder":
                    RunWithAnimation("soundrecorder.exe", "Starting Voice Recorder...");
                    break;
                case "Camera":
                    RunWithAnimation("microsoft.windows.camera:", "Starting Camera...", useShell: true);
                    break;
                case "Media Player":
                    RunWithAnimation("wmplayer.exe", "Starting Media Player...");
                    break;
                case "Back":
                    return;
            }
            
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ {tool.Replace("Back", "")} launched successfully![/]");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to launch {tool}: {ex.Message}");
        }
        
        WaitForContinue();
    }
    
    private static void OpenAdministrativeTools()
    {
        Console.Clear();
        
        var tool = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]Administrative Tools[/]")
                .PageSize(GraphicSettings.PageSize)
                .AddChoices([
                    "Task Manager",
                    "Registry Editor",
                    "Disk Management",
                    "Computer Management",
                    "Device Manager",
                    "Services",
                    "Performance Monitor",
                    "Event Viewer",
                    "Task Scheduler",
                    "Local Users & Groups",
                    "Local Security Policy",
                    "System Configuration",
                    "Disk Cleanup",
                    "Back"
                ]));
        
        try
        {
            switch (tool)
            {
                case "Task Manager":
                    RunWithAnimation("taskmgr.exe", "Starting Task Manager...");
                    break;
                case "Registry Editor":
                    RunWithAnimation("regedit.exe", "Starting Registry Editor...");
                    break;
                case "Disk Management":
                    RunWithAnimation("mmc.exe", "diskmgmt.msc", "Starting Disk Management...");
                    break;
                case "Computer Management":
                    RunWithAnimation("mmc.exe", "compmgmt.msc", "Starting Computer Management...");
                    break;
                case "Device Manager":
                    RunWithAnimation("mmc.exe", "devmgmt.msc", "Starting Device Manager...");
                    break;
                case "Services":
                    RunWithAnimation("mmc.exe", "services.msc", "Starting Services...");
                    break;
                case "Performance Monitor":
                    RunWithAnimation("mmc.exe", "perfmon.msc", "Starting Performance Monitor...");
                    break;
                case "Event Viewer":
                    RunWithAnimation("mmc.exe", "eventvwr.msc", "Starting Event Viewer...");
                    break;
                case "Task Scheduler":
                    RunWithAnimation("taskschd.msc", "", "Starting Task Scheduler...");
                    break;
                case "Local Users & Groups":
                    RunWithAnimation("mmc.exe", "lusrmgr.msc", "Starting Local Users & Groups...");
                    break;
                case "Local Security Policy":
                    RunWithAnimation("secpol.msc", "", "Starting Local Security Policy...");
                    break;
                case "System Configuration":
                    RunWithAnimation("msconfig.exe", "Starting System Configuration...");
                    break;
                case "Disk Cleanup":
                    RunWithAnimation("cleanmgr.exe", "Starting Disk Cleanup...");
                    break;
                case "Back":
                    return;
            }
            
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ {tool.Replace("🔙 Back", "")} launched successfully![/]");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to launch {tool}: {ex.Message}");
            if (ex.Message.Contains("gpedit.msc") || ex.Message.Contains("secpol.msc"))
            {
                AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Note: This tool may require Windows Pro/Enterprise edition.[/]");
            }
        }
        
        WaitForContinue();
    }
    
    private static void OpenControlPanel()
    {
        Console.Clear();
        
        var panelItem = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]Control Panel Items[/]")
                .PageSize(GraphicSettings.PageSize)
                .AddChoices([
                    "Control Panel",
                    "Programs & Features",
                    "Windows Defender Firewall",
                    "Display Settings",
                    "Sound Settings",
                    "Internet Options",
                    "User Accounts",
                    "Date & Time",
                    "Power Options",
                    "Region & Language",
                    "Network Connections",
                    "Devices & Printers",
                    "Security & Maintenance",
                    "Back"
                ]));
        
        try
        {
            switch (panelItem)
            {
                case "Control Panel":
                    RunWithAnimation("control.exe", "Opening Control Panel...");
                    break;
                case "Programs & Features":
                    RunWithAnimation("control.exe", "appwiz.cpl", "Opening Programs & Features...");
                    break;
                case "Windows Defender Firewall":
                    RunWithAnimation("control.exe", "firewall.cpl", "Opening Windows Defender Firewall...");
                    break;
                case "Display Settings":
                    RunWithAnimation("control.exe", "desk.cpl", "Opening Display Settings...");
                    break;
                case "Sound Settings":
                    RunWithAnimation("control.exe", "mmsys.cpl", "Opening Sound Settings...");
                    break;
                case "Internet Options":
                    RunWithAnimation("control.exe", "inetcpl.cpl", "Opening Internet Options...");
                    break;
                case "User Accounts":
                    RunWithAnimation("control.exe", "nusrmgr.cpl", "Opening User Accounts...");
                    break;
                case "Date & Time":
                    RunWithAnimation("control.exe", "timedate.cpl", "Opening Date & Time...");
                    break;
                case "Power Options":
                    RunWithAnimation("control.exe", "powercfg.cpl", "Opening Power Options...");
                    break;
                case "Region & Language":
                    RunWithAnimation("control.exe", "intl.cpl", "Opening Region & Language...");
                    break;
                case "Network Connections":
                    RunWithAnimation("control.exe", "ncpa.cpl", "Opening Network Connections...");
                    break;
                case "Devices & Printers":
                    RunWithAnimation("control.exe", "printers", "Opening Devices & Printers...");
                    break;
                case "Security & Maintenance":
                    RunWithAnimation("control.exe", "wscui.cpl", "Opening Security & Maintenance...");
                    break;
                case "Back":
                    return;
            }
            
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ {panelItem.Replace("🔙 Back", "")} opened successfully![/]");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to open {panelItem}: {ex.Message}");
        }
        
        WaitForContinue();
    }
    private static void OpenCustomFile()
    {
        Console.Clear();
        
        AnsiConsole.Write(
            new Panel($"[{GraphicSettings.SecondaryColor}]Custom File Launcher[/]")
                .BorderColor(GraphicSettings.GetThemeColor) 
                .Padding(1, 1, 1, 1));
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]How would you like to open a file?[/]")
                .AddChoices([
                    "Enter Full Path",
                    "Browse File",
                    "Back"
                ]));
        
        if (choice == "🔙 Back") return;
        
        try
        {
            string path = "";
            
            if (choice == "Enter Full Path")
            {
                path = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter full path to file:[/]")
                        .PromptStyle(GraphicSettings.AccentColor)
                        .Validate(p =>
                        {
                            if (string.IsNullOrWhiteSpace(p))
                                return ValidationResult.Error("[red]Path cannot be empty[/]");
                            
                            return ValidationResult.Success();
                        }));
                
                path = path.Trim('"');
                
                if (!File.Exists(path) && !Directory.Exists(path))
                {

                    if (!AnsiConsole.Confirm($"[{GraphicSettings.SecondaryColor}]File/directory '{Path.GetFileName(path)}' may not exist. Continue anyway?[/]", false))
                    {
                        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]]Operation cancelled.[/]");
                        WaitForContinue();
                        return;
                    }
                }
            }
            else if (choice == "Browse File")
            {
                AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Please enter path manually or drag-and-drop file here:[/]");
                path = Console.ReadLine()?.Trim('"');
                
                if (string.IsNullOrWhiteSpace(path))
                {
                    AnsiConsole.MarkupLine("[red]No path provided.[/]");
                    WaitForContinue();
                    return;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(path))
            {
   
                AnsiConsole.Status()
                    .Start($"Opening {Path.GetFileName(path)}...", ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        ctx.SpinnerStyle(Style.Parse(GraphicSettings.AccentColor));
                        Thread.Sleep(800);
                    });
                
                bool exists = File.Exists(path) || Directory.Exists(path);
                
                if (exists)
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ Successfully opened:[/] [{GraphicSettings.SecondaryColor}]{path}[/]");
                }
                else
                {
         
                    var fileInPath = FindInPath(path);
                    if (fileInPath != null)
                    {
                        Process.Start(fileInPath);
                        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ Found in PATH and opened:[/] [{GraphicSettings.SecondaryColor}]{fileInPath}[/]");
                    }
                    else
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ Attempting to open:[/] [{GraphicSettings.SecondaryColor}]{path}[/]");
                        }
                        catch
                        {
                            AnsiConsole.MarkupLine($"[red]✗ File not found: {path}[/]");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to open file: {ex.Message}");
        }
        
        WaitForContinue();
    }
    
    private static void RunWithAnimation(string fileName, string message)
    {
        AnsiConsole.Status()
            .Start(message, ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse(GraphicSettings.AccentColor));
                Thread.Sleep(800);
                Process.Start(fileName);
            });
    }
    
    private static void RunWithAnimation(string fileName, string arguments, string message)
    {
        AnsiConsole.Status()
            .Start(message, ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse(GraphicSettings.AccentColor));
                Thread.Sleep(800);
                Process.Start(fileName, arguments);
            });
    }
    
    private static void RunWithAnimation(string fileName, string message, bool useShell)
    {
        AnsiConsole.Status()
            .Start(message, ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse(GraphicSettings.AccentColor));
                Thread.Sleep(800);
                Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = useShell });
            });
    }
    
    private static void RunCommandWithOutput(string command, string arguments)
    {
        Console.Clear();
        
        AnsiConsole.Write(
            new Panel($"[{GraphicSettings.SecondaryColor}]{command} {arguments}[/]")
                .BorderColor(GraphicSettings.GetThemeColor)
                .Padding(1, 1, 1, 1));
        
        try
        {
            Process process = new();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            

            var table = new Table()
                .BorderColor(GraphicSettings.GetThemeColor)
                .Border(TableBorder.HeavyEdge)
                .AddColumn(new TableColumn($"[{GraphicSettings.NeutralColor}]Output[/]").LeftAligned());
            
            foreach (var line in output.Split('\n')) 
            {
                if (!string.IsNullOrWhiteSpace(line))
                    table.AddRow($"[{GraphicSettings.SecondaryColor}]{line.Trim()}[/]");
            }
            
            AnsiConsole.Write(table);
            
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]✓ Command executed successfully (Exit code: {process.ExitCode})[/]");
        }
        catch (Exception ex)
        {
            ShowError($"Command failed: {ex.Message}");
        }
    }
    
    private static string FindInPath(string fileName)
    {
        if (File.Exists(fileName)) return fileName;
        
        var pathDirs = Environment.GetEnvironmentVariable("PATH")?.Split(';');
        if (pathDirs != null)
        {
            foreach (var dir in pathDirs)
            {
                if (Directory.Exists(dir))
                {
                    var fullPath = Path.Combine(dir, fileName);
                    if (File.Exists(fullPath))
                        return fullPath;
                        

                    if (!fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        fullPath = Path.Combine(dir, fileName + ".exe");
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                }
            }
        }
        
        return null;
    }
    
    private static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗ {message}[/]");
    }
    
    private static void WaitForContinue()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
        Console.ReadKey();
    }
}