using System;
using Spectre.Console;
using Task_Manager_T4;
using System.IO;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Task Manager T4";

        if (args.Length > 0 && args[0] == "--fix-keyboard")
        {
            Other.Keyboard.FixKeyboard();
            return;
        }
        string fontPath = Path.Combine(AppContext.BaseDirectory, "resources", "fonts", "Speed.flf");
        while (true)
        {
            AnsiConsole.Clear();

            if (File.Exists(fontPath))
            {
                var font = FigletFont.Load(fontPath);
                AnsiConsole.Write(
                    new FigletText(font, "tm T4")
                        .Centered()
                        .Color(Color.Orange1));
            }
            else
            {
                AnsiConsole.Write(new FigletText("tm T4").Centered().Color(Color.Orange1));
            }
            var sysInfo = new Table()
                .Border(TableBorder.HeavyEdge)
                .BorderColor(Color.Orange1)
                .AddColumn("Параметр")
                .AddColumn("Значение");

            sysInfo.AddRow("[orange1]OS[/]", Environment.OSVersion.ToString());
            sysInfo.AddRow("[orange1]User[/]", Environment.UserName);
            sysInfo.AddRow("[orange1]Machine[/]", Environment.MachineName);
            sysInfo.AddRow("[orange1]CPU Cores[/]", Environment.ProcessorCount.ToString());

            AnsiConsole.Write(
                new Panel(sysInfo)
                    .Header("[bold white] System Dashboard [/]")
                    .Expand()
                    .BorderColor(Color.White));
            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]{GraphicSettings.AppVersion} Press any key to continue[/]").RuleStyle(GraphicSettings.AccentColor));
            Console.ReadKey();
            await Function_list();
        }
    }

    public static async Task Function_list()
    {
        Console.Clear();
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{GraphicSettings.AccentColor}]Select an option:[/]")
                    .PageSize(GraphicSettings.PageSize)
                    .MoreChoicesText($"[{GraphicSettings.NeutralColor}](Move up and down to reveal more options)[/]")
                    .AddChoices(
                    [
                        "Process Management",
                        "Service Manager",
                        "Privacy settings",
                        "Startup Manager",
                        "System Information",
                        "Drives",
                        "Registry",
                        "Show System Load",
                        "Check Temperature",
                        "Benchmark",
                        "Program Launcher",
                        "File and folder manager",
                        "Program Manager",
                        "Network",
                        "Windows Optimization",
                        "Other",
                        "Graphic Settings",
                        "OpenMe",
                        "Exit"
                    ]));

            switch (choice)
            {
                case "Privacy settings":
                    EnablingPrivacy.ShowPrivacyMenu();
                    break;
                case "Windows Optimization":
                    WindowsOptimization.MainMenuOptimization();
                    break;
                case "Process Management":
                    Process_management.GetProcces();
                    break;
                case "System Information":
                    GetInfoPc.Main_Information_Collection();
                    break;
                case "Program Launcher":
                    OpenProgram.OpenPrograms();
                    break;
                case "Show System Load":
                    Load.MainMenu();
                    Console.Clear();
                    break;
                case "Startup Manager":
                    await StartUpManager.ShowStartupManagerUI();
                    break;
                case "Check Temperature":
                    AdvancedTemperatureMonitor.ShowAllTemperatures();
                    Console.Clear();
                    break;
                case "Service Manager":
                    ServiceManagerUI.ShowServicesMenu();
                    break;
                case "Other":
                    Other.PrintAllOtherFunctions();
                    Console.Clear();
                    break;
                case "Drives":
                    DriveManager.Main_Menu_Drives();
                    break;
                case "Benchmark":
                    await SystemBenchmark.ShowBenchmarkMenu(); //che za huynya 
                    break;
                case "OpenMe":
                    Rain.ShowReadMeWithRain();
                    Console.Clear();
                    break;
                case "File and folder manager":
                    Console.Clear();
                    MainFF.PrintFunctions();
                    break;
                case "Registry":
                    Management_Registry.Main_Menu_Registry();
                    Console.Clear();
                    break;
                case "Program Manager":
                    ProgramManager.MainMenuProgramManager();
                    break;      
                case "Graphic Settings":
                    GraphicSettings.ChangeTheme();
                    break;
                case "Network":
                    await NetWork.MainMenuNetWork();
                    break;          
                case "Exit":
                    Environment.Exit(0);
                    break;
            }
        }
    }

}