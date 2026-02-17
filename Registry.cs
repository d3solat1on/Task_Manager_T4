using Microsoft.Win32;
using Spectre.Console;
using System;
using Task_Manager_T4;

class Management_Registry
{

    public static void Main_Menu_Registry()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]REGISTRY MANAGER[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());
            AnsiConsole.MarkupLine("[bold red]ВНИМАНИЕ:[/] РЕДАКТИРОВАНИЕ РЕЕСТРА МОЖЕТ ПОВРЕДИТЬ СИСТЕМУ.\n");
            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Данный модуль тестовый. Будет дополнятся позже[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{GraphicSettings.SecondaryColor}]ВЫБЕРИТЕ ДЕЙСТВИЕ:[/]")
                    .PageSize(GraphicSettings.PageSize)
                    .AddChoices([
                        "System Info (Read Only)",
                        "View Startup Programs",
                        "Toggle Clock Seconds (Tweak)",
                        "Назад"
                    ]));

            switch (choice)
            {
                case "System Info (Read Only)":
                    ShowSystemInfo();
                    break;
                case "View Startup Programs":
                    ShowStartupRegistry();
                    break;
                case "Toggle Clock Seconds (Tweak)":
                    ToggleSecondsInClock();
                    break;
                case "Назад":
                     return;
            }
        }
    }

    private static void ShowSystemInfo()
    {
        Console.Clear();

        string winPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        string biosPath = @"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS";
        string cpuPath = @"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0";
        string gpuPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0001";

        try
        {
            using RegistryKey winKey = Registry.LocalMachine.OpenSubKey(winPath);

            object model_motherboard = Registry.GetValue(biosPath, "BaseBoardProduct", null);
            object vendor_motherboard = Registry.GetValue(biosPath, "SystemManufacturer", null);

            object name_processor = Registry.GetValue(cpuPath, "ProcessorNameString", null);
            object vendor_processor = Registry.GetValue(cpuPath, "VendorIdentifier", null);
            
            object model_videocard = Registry.GetValue(gpuPath, "DriverDesc", null);
            object vendor_videocard = Registry.GetValue(gpuPath,"ProviderName", null);

            if (winKey != null)
            {
                var productName = winKey.GetValue("ProductName");
                var displayVersion = winKey.GetValue("DisplayVersion");

                var table = new Table().BorderColor(GraphicSettings.GetThemeColor).Border(TableBorder.Rounded); 
                table.AddColumn($"[{GraphicSettings.SecondaryColor}]ПАРАМЕТР[/]");
                table.AddColumn($"[{GraphicSettings.SecondaryColor}]ЗНАЧЕНИЕ[/]");

                table.AddRow($"[{GraphicSettings.SecondaryColor}]Windows Product[/]", productName?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Version[/]", displayVersion?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Motherboard[/]", model_motherboard?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Vendor[/]", vendor_motherboard?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Processor[/]", name_processor?.ToString()?.Trim() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Vendor[/]", vendor_processor?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Videocard[/]", model_videocard?.ToString() ?? "N/A");
                table.AddRow($"[{GraphicSettings.SecondaryColor}]Vendor[/]", vendor_videocard?.ToString() ?? "N/A");

                AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]SYSTEM_HARDWARE_REPORT[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red] Ошибка доступа к ветке CurrentVersion[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red] Произошла ошибка:[/] {ex.Message}");
        }

        AnsiConsole.MarkupLine($"\n[{GraphicSettings.NeutralColor}]Нажмите любую клавишу для возврата...[/]");
        Console.ReadKey();
    }

    private static void ShowStartupRegistry()
    {
        Console.Clear();
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
        if (key != null)
        {
            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Startup Apps (Registry)[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());
            var table = new Table().BorderColor(GraphicSettings.GetThemeColor); 
            table.AddColumn("App Name");
            table.AddColumn("Path");

            foreach (string valueName in key.GetValueNames())
            {
                table.AddRow(valueName, key.GetValue(valueName)?.ToString() ?? "");
            }
            AnsiConsole.Write(table);
        }
        Console.ReadKey();
    }
    private static void ToggleSecondsInClock()
    {
        Console.Clear();
        const string subKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        const string valueName = "ShowSecondsInSystemClock";

        try
        {
            // Открываем ветку с правами на запись (true)
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true);
            if (key != null)
            {
                // Получаем текущее значение (0 - выкл, 1 - вкл). По умолчанию 0.
                int currentValue = Convert.ToInt32(key.GetValue(valueName, 0));
                int newValue = currentValue == 0 ? 1 : 0;

                key.SetValue(valueName, newValue, RegistryValueKind.DWord);

                AnsiConsole.MarkupLine(newValue == 1
                    ? "[green] Секунды в часах включены![/]"
                    : "[yellow] Секунды в часах выключены![/]");

            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Примечание: Чтобы изменения вступили в силу, нужно перезапустить Проводник (Explorer).[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red] Не удалось изменить реестр:[/] {Markup.Escape(ex.Message)}");
        }
        Console.ReadKey();
    }
}