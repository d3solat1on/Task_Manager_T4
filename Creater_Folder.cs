using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text;
using ProjectT4;

class CreaterFolder
{
    static string GetDesktopPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    // Вариант 2: Папка в документах пользователя
    static string GetDocumentsPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    // Вариант 3: Папка в AppData
    static string GetAppDataPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }

    // Вариант 4: Папка в LocalAppData 
    static string GetLocalAppDataPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }

    public static string CreateReportFolder(string folderName = "test001111111")
    {
        // Выбираем место для сохранения
        string basePath = GetDesktopPath(); // или GetDocumentsPath()
        string folderPath = Path.Combine(basePath, folderName);
        
        try
        {
            // Создаем папку если ее нет
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"[+] Папка создана: {folderPath}");
            }
            else
            {
                Console.WriteLine($"[+] Папка уже существует: {folderPath}");
            }
            
            return folderPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Не удалось создать папку: {ex.Message}");
            
            // Пробуем альтернативное место если не удалось создать на рабочем столе
            string fallbackPath = Path.Combine(GetLocalAppDataPath(), folderName);
            Directory.CreateDirectory(fallbackPath);
            Console.WriteLine($"[+] Папка создана в альтернативном месте: {fallbackPath}");
            
            return fallbackPath;
        }
    }

    // public static void CreaterFiles()
    // {
    //     try
    //     {
    //         string path = GetDesktopPath();
    //         // Create the file, or overwrite if the file exists.
    //         using (FileStream fs = File.Create(path))
    //         {
    //             byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
    //             // Add some information to the file.
    //             fs.Write(info, 0, info.Length);
    //         }
    //     }
    //     catch(Exception ex)
    //     {
    //         Console.WriteLine(ex.ToString());
    //     }
    // }
}