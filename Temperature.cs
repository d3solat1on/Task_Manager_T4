using System;
using LibreHardwareMonitor.Hardware;
using Spectre.Console;
namespace Task_Manager_T4;

public class AdvancedTemperatureMonitor
{
    private static Computer _computer;
    
    public static void Initialize()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsStorageEnabled = true
        };
        
        _computer.Open();
    }
    
    public static void ShowAllTemperatures()
    {
        Initialize();
        
        var table = new Table()
            .Title($"[{GraphicSettings.SecondaryColor}]Hardware Temperatures[/]")
            .BorderColor(GraphicSettings.GetThemeColor)
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Hardware[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Sensor[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Temperature[/]").RightAligned())
            .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Status[/]").Centered());
        
        foreach (var hardware in _computer.Hardware)
        {
            // Принудительное обновление данных для всех компонентов
            hardware.Update();
            
            // Для процессоров и материнских плат иногда нужно обновить вложенные объекты
            foreach (var subHardware in hardware.SubHardware) subHardware.Update();

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                {
                    string sensorName = sensor.Name.ToLower();
                    
                    // ФИЛЬТРАЦИЯ: Пропускаем датчики лимитов (Warning, Critical, Max)
                    if (sensorName.Contains("warning") || 
                        sensorName.Contains("critical") || 
                        sensorName.Contains("limit") ||
                        sensorName.Contains("max"))
                    {
                        continue;
                    }

                    double temp = sensor.Value.Value;
                    string status = GetTemperatureStatus(temp, hardware.HardwareType);
                    Color color = GetStatusColor(status);
                    
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{hardware.Name}[/]",
                        $"[{GraphicSettings.SecondaryColor}]{sensor.Name}[/]",
                        $"[{color.ToMarkup()}]{temp:F1}°C[/]",
                        $"[{color.ToMarkup()}]{status}[/]"
                    );
                }
            }
        }
        
        if (table.Rows.Count == 0)
        {
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]]Реальные датчики температуры не найдены.[/]");
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Убедитесь, что программа запущена от имени АДМИНИСТРАТОРА.[/]");
        }
        else
        {
            AnsiConsole.Write(table);
        }
        
        _computer.Close();
        AnsiConsole.MarkupLine($"[{GraphicSettings.NeutralColor}]Нажмите любую клавишу для выхода...[/]");
        Console.ReadKey();
    }
    
    private static string GetTemperatureStatus(double temp, HardwareType type)
    {
        // Настраиваем пороги под разные устройства
        double threshold = type switch
        {
            HardwareType.Cpu => 80,
            HardwareType.GpuNvidia => 83, // Стандарт для 40-й серии
            HardwareType.GpuAmd => 85,
            HardwareType.Storage => 55, // Диски более чувствительны к перегреву
            HardwareType.Motherboard => 70,
            _ => 75
        };
        
        if (temp > threshold + 10) return "CRITICAL";
        if (temp > threshold) return "HIGH";
        if (temp > threshold - 20) return "NORMAL";
        return "LOW";
    }
    
    private static Color GetStatusColor(string status)
    {
        return status switch
        {
            "CRITICAL" => Color.Red,
            "HIGH" => Color.Yellow,
            "NORMAL" => Color.Green,
            "LOW" => Color.Blue,
            _ => Color.White
        };
    }
}