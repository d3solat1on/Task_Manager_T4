namespace ProjectT4;
using Spectre.Console;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TextDisplay
{
    public static void TypeWrite(string text, int delay = 1)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }
}


public class Img
{
    private static readonly string[] image = {
            " @@@@@                                        @@@@@",
            "@@@@@@@           @@@@@@@@@@@@@@@            @@@@@@@",
            "@@@@@@@@       @@@@@@@@@@@@@@@@@@@        @@@@@@@@",
            "@@@@@     @@@@@@@@@@@@@@@@@@@@@     @@@@@",
            "     @@@@@  @@@@@@@@@@@@@@@@@@@@@@@  @@@@@",
            "       @@  @@@@@@@@@@@@@@@@@@@@@@@@@  @@",
            "         @@@@@@@    @@@@@@    @@@@@@",
            "           @@@@@@      @@@@      @@@@@",
            "            @@@@@@      @@@@      @@@@@",
            "           @@@@@@    @@@@@@    @@@@@",
            "             @@@@@@@@@@@  @@@@@@@@@@",
            "              @@@@@@@@@@  @@@@@@@@@",
            "              @@   @@@@@@@@@@@@@@@@@   @@",
            "           @@@@  @@@@ @ @ @ @ @@@@  @@@@",
            "           @@@@@   @@@ @ @ @ @ @@@   @@@@@",
            "          @@@@@      @@@@@@@@@@@@@      @@@@@",
            "        @@@@          @@@@@@@@@@@          @@@@",
            "      @@@@@              @@@@@@@              @@@@@",
            "  @@@@@@@                                 @@@@@@@",
            " @@@@@                                   @@@@@" // Обратите внимание: здесь должно быть 5 @
        };

    public static void PrintFallingCharsEffect(int delay = 1)
    {
        Console.CursorVisible = false;
        // Позиция начала черепа (после надписи)
        int startTop = Console.CursorTop;

        // Создаем массив для отслеживания позиций символов
        List<(int targetRow, int col, char ch, int currentRow)> fallingChars =
            new List<(int, int, char, int)>();

        Random random = new Random();

        // Собираем все символы, которые нужно вывести
        for (int row = 0; row < image.Length; row++)
        {
            for (int col = 0; col < image[row].Length; col++)
            {
                if (image[row][col] != ' ')
                {
                    // Начальная позиция выше экрана
                    fallingChars.Add((row, col, image[row][col], -random.Next(1, 10)));
                }
            }
        }

        // Перемешиваем символы
        Shuffle(fallingChars, random);

        bool allLanded;
        do
        {
            allLanded = true;
            Console.SetCursorPosition(0, startTop);

            // Очищаем только область черепа (не трогаем надпись)
            for (int i = 0; i < image.Length; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            // Используем обычный for вместо foreach
            for (int i = 0; i < fallingChars.Count; i++)
            {
                var (targetRow, col, ch, currentRow) = fallingChars[i];

                if (currentRow < targetRow)
                {
                    allLanded = false;

                    // Символ еще падает
                    if (currentRow >= 0 && currentRow < image.Length)
                    {
                        Console.SetCursorPosition(col, startTop + currentRow);
                        Console.Write(ch);
                    }

                    // Увеличиваем текущую строку
                    currentRow++;
                    fallingChars[i] = (targetRow, col, ch, currentRow);
                }
                else
                {
                    // Символ на месте
                    Console.SetCursorPosition(col, startTop + targetRow);
                    Console.Write(ch);
                }
            }

            Thread.Sleep(delay);

        } while (!allLanded);

        Console.SetCursorPosition(0, startTop + image.Length);
        Console.CursorVisible = true;
    }

    public static void BlinkSkull(int blinkCount = 20, int blinkDelay = 75)
    {
        Console.CursorVisible = false;

        // Позиция начала черепа (предполагаем, что он уже нарисован)
        int currentTop = Console.CursorTop;
        int skullStartTop = Math.Max(0, currentTop - image.Length);

        // Сохраняем оригинальный цвет
        ConsoleColor originalColor = Console.ForegroundColor;

        // Массив цветов для мерцания
        ConsoleColor[] colors = {
                ConsoleColor.White,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Green,
                ConsoleColor.Magenta,
                ConsoleColor.Red,
                ConsoleColor.Gray,
                ConsoleColor.DarkYellow,
                ConsoleColor.Blue,
                ConsoleColor.DarkCyan
            };

        Random random = new Random();
            for (int i = 0; i < blinkCount; i++)
            {
                // Выбираем случайный цвет для черепа
                Console.ForegroundColor = colors[random.Next(colors.Length)];

                // Перерисовываем череп
                Console.SetCursorPosition(0, skullStartTop);
                foreach (string line in image)
                {
                    Console.WriteLine(line);
                }

                Thread.Sleep(blinkDelay);
            }

            // Возвращаем белый цвет
            Console.ForegroundColor = ConsoleColor.White;

            // Перерисовываем череп в белом цвете
            Console.SetCursorPosition(0, skullStartTop);
            foreach (string line in image)
            {
                Console.WriteLine(line);
            }

            Console.SetCursorPosition(0, skullStartTop + image.Length);
            Console.CursorVisible = true;
            Console.ForegroundColor = originalColor;
        }
    private static void Shuffle<T>(List<T> list, Random random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static void OpenMe()
    {
        TextDisplay.TypeWrite("skull =)", 15);
        BlinkSkull();
        PrintFallingCharsEffect();
        TextDisplay.TypeWrite("Oops, it doesn't seem to be working. =(");
        TextDisplay.TypeWrite("But it's okay");
        TextDisplay.TypeWrite("This programm made by me =)");
        TextDisplay.TypeWrite("Press any key to return main menu.");
        Console.ReadLine();
    }
}
public class Rain
{
    public static void ShowReadMeWithRain()
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        // Сохраняем оригинальный цвет консоли
        var originalForeground = Console.ForegroundColor;
        var originalBackground = Console.BackgroundColor;
        
        try
        {
            // Устанавливаем темный фон для эффекта ночного дождя
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            
            // Создаем эффект дождя в отдельном потоке
            var cancellationTokenSource = new CancellationTokenSource();
            var rainTask = Task.Run(() => PrintRain(cancellationTokenSource.Token));
            
            // Показываем инструкцию
            Console.SetCursorPosition(Console.WindowWidth / 2 - 15, 5);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[ PRESS ANY KEY TO CONTINUE ]");
            
            // Ждем нажатия клавиши
            Console.ReadKey(true);
            
            // Останавливаем дождь
            cancellationTokenSource.Cancel();
            rainTask.Wait(1000); // Ждем завершения задачи
            
            // Очищаем экран и показываем информацию
            Console.Clear();
            ShowReadMeInformation();
            
            // Ждем нажатия клавиши для возврата в меню
            Console.WriteLine("\n\nPress any key to return to main menu...");
            Console.ReadKey(true);
        }
        finally
        {
            // Восстанавливаем оригинальные цвета
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
            Console.CursorVisible = true;
        }
    }
    
    private static void PrintRain(CancellationToken cancellationToken)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        
        // Количество капель
        int rainCount = Math.Min(width, 80); // Ограничиваем количество
        int[] x = new int[rainCount];
        int[] y = new int[rainCount];
        char[] chars = new char[rainCount]; // Разные символы для дождя
        ConsoleColor[] colors = new ConsoleColor[rainCount]; // Разные цвета
        Random rand = new Random();
        
        // Начальное заполнение
        for (int i = 0; i < rainCount; i++)
        {
            x[i] = rand.Next(0, width);
            y[i] = rand.Next(0, height);
            chars[i] = GetRandomRainChar(rand);
            colors[i] = GetRandomRainColor(rand);
        }
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                for (int i = 0; i < rainCount; i++)
                {
                    // Проверяем, не была ли нажата клавиша
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    
                    // Стираем старую каплю (рисуем пробел)
                    if (y[i] >= 0 && y[i] < height && x[i] >= 0 && x[i] < width)
                    {
                        Console.SetCursorPosition(x[i], y[i]);
                        Console.Write(" ");
                    }
                    
                    // Двигаем каплю вниз
                    y[i]++;
                    
                    // Если капля упала за предел экрана, переносим её наверх
                    if (y[i] >= height)
                    {
                        y[i] = 0;
                        x[i] = rand.Next(0, width);
                        chars[i] = GetRandomRainChar(rand);
                        colors[i] = GetRandomRainColor(rand);
                    }
                    
                    // Рисуем каплю в новой позиции
                    if (y[i] >= 0 && y[i] < height && x[i] >= 0 && x[i] < width)
                    {
                        Console.SetCursorPosition(x[i], y[i]);
                        Console.ForegroundColor = colors[i];
                        Console.Write(chars[i]);
                    }
                }
                
                // Скорость анимации
                Thread.Sleep(30);
            }
            catch (Exception)
            {
                // Игнорируем ошибки (например, изменение размера окна)
                break;
            }
        }
    }
    
    private static char GetRandomRainChar(Random rand)
    {
        char[] rainChars = { '|', '│', '┃', '╽', '╿', '║', ':', '\'', '.' };
        return rainChars[rand.Next(rainChars.Length)];
    }
    
    private static ConsoleColor GetRandomRainColor(Random rand)
    {
        ConsoleColor[] rainColors = 
        { 
            ConsoleColor.Cyan, 
            ConsoleColor.Blue, 
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkBlue,
            ConsoleColor.White,
            ConsoleColor.Gray
        };
        return rainColors[rand.Next(rainColors.Length)];
    }
    
    private static void ShowReadMeInformation()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        // Красивый заголовок
        string title = @"
╔══════════════════════════════════════════════════════╗
║                 SYSTEM MONITOR v1.0                  ║
║                    READ ME / HELP                    ║
╚══════════════════════════════════════════════════════╝";


        Console.WriteLine(title);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║This programm made by me =)                           ║");
        Console.WriteLine("║Telegram: https://t.me/+URXdMVxOgrdiMDRi              ║");
        Console.WriteLine("║Steam: https://steamcommunity.com/id/d3s0lation       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
    }
}