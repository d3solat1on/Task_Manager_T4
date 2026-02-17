using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Spectre.Console;

namespace Task_Manager_T4;

class SystemBenchmark
{
    public static async Task ShowBenchmarkMenu()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]System Performance Benchmark[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[{GraphicSettings.SecondaryColor}]Выберите тип теста:[/]")
                .AddChoices(["CPU Stress Test (Parallel)", "RAM Speed Test", "Назад"]));

        if (choice == "CPU Stress Test (Parallel)") await RunCpuBenchmark();
        else if (choice == "RAM Speed Test") await RunRamBenchmark();
        else if (choice == "Назад") await Program.Function_list();
    }

    private static async Task RunCpuBenchmark()
    {
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Запуск теста CPU... Все ядра будут нагружены на 5 секунд.[/]");

        Stopwatch sw = Stopwatch.StartNew();
        long operations = 0;


        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .Start("Выполнение вычислений...", ctx =>
            {
                DateTime endTime = DateTime.Now.AddSeconds(5);

                Parallel.For(0, Environment.ProcessorCount, i =>
                {
                    while (DateTime.Now < endTime)
                    {
                        double x = Math.Sqrt(Math.Pow(123.45, 67.89));
                        System.Threading.Interlocked.Increment(ref operations);
                    }
                });
            });

        sw.Stop();
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Тест завершен![/]");
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Выполнено математических операций:[/] [{GraphicSettings.SecondaryColor}]{operations:N0}[/]");
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Относительный балл (Ops/sec):[/] [{GraphicSettings.SecondaryColor}]{operations / 5:N0}[/]");
        if (AnsiConsole.Confirm($"[{GraphicSettings.AccentColor}]Do you want to export this data to a file??[/]", true))
        {
            ExportCpuBenchmarkToFile(operations, sw.Elapsed.TotalSeconds);
        }
        AnsiConsole.MarkupLine("Press any key to return.");
        Console.ReadLine();
        await ShowBenchmarkMenu();
    }

    private static void ExportCpuBenchmarkToFile(long operations, double totalSeconds)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "SystemReport");

        if (!Directory.Exists(folderPath))
        
        {
            Directory.CreateDirectory(folderPath);
        }

        string reportFile = Path.Combine(folderPath, $"CPU_Benchmark_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        using StreamWriter sw = new(reportFile);
        sw.WriteLine("=== CPU Benchmark REPORT ===");
        sw.WriteLine($"Generated: {DateTime.Now}");
        sw.WriteLine($"Computer: {Environment.MachineName}");
        sw.WriteLine(new string('=', 80));
        sw.WriteLine($"Выполнено математических операций:{operations}");
        sw.WriteLine($"Относительный балл (Ops/sec): {operations / totalSeconds}");
        sw.WriteLine(new string('=', 80));
        sw.WriteLine("Report saved successfully!");
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]CPU benchmark exported to:[/] [{GraphicSettings.SecondaryColor}]{reportFile}[/]");
    }

    private static void ExportRamBenchmarkToFile(double writeSpeed, double readSpeed)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "SystemReport");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string reportFile = Path.Combine(folderPath, $"RAM_Benchmark_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        using StreamWriter sw = new(reportFile);
        sw.WriteLine("=== RAM Benchmark REPORT ===");
        sw.WriteLine($"Generated: {DateTime.Now}");
        sw.WriteLine($"Computer: {Environment.MachineName}");
        sw.WriteLine(new string('=', 80));
        sw.WriteLine($"Скорость записи: {writeSpeed:F2} MB/s");
        sw.WriteLine($"Скорость чтения: {readSpeed:F2} MB/s");
        sw.WriteLine(new string('=', 80));
        sw.WriteLine("Report saved successfully!");
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]RAM benchmark exported to:[/] [{GraphicSettings.SecondaryColor}]{reportFile}[/]");
    }

    private static async Task RunRamBenchmark()
    {
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Запуск теста RAM (Запись/Чтение 1 ГБ)...[/]");

        const int size = 1024 * 1024 * 256;
        int[] array = new int[size];
        Stopwatch sw = new();

        AnsiConsole.Status()
            .Start("Тестирование скорости записи...", ctx =>
            {
                sw.Start();
                for (int i = 0; i < size; i++) array[i] = i;
                sw.Stop();
            });

        double writeSpeed = 1024.0 / sw.Elapsed.TotalSeconds;
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Скорость записи:[/] [{GraphicSettings.SecondaryColor}]{writeSpeed:F2} MB/s[/]");

        sw.Restart();
        AnsiConsole.Status()
            .Start("Тестирование скорости чтения...", ctx =>
            {
                long sum = 0;
                for (int i = 0; i < size; i++) sum += array[i];
                sw.Stop();
            });

        double readSpeed = 1024.0 / sw.Elapsed.TotalSeconds;
        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Скорость чтения: {readSpeed:F2} MB/s[/]");


        array = null;
        GC.Collect();
        if (AnsiConsole.Confirm($"[{GraphicSettings.AccentColor}]Do you want to export this data to a file??[/]", true))
        {
            ExportRamBenchmarkToFile(writeSpeed, readSpeed);
        }
        AnsiConsole.MarkupLine("Press any key to return.");
        Console.ReadLine();
        await ShowBenchmarkMenu();
    }
}