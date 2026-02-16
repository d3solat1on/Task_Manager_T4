using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using Spectre.Console;
namespace Task_Manager_T4;

public class Other
{
    public static void PrintAllOtherFunctions()
    {
        while (true)
        {
            Console.Clear();

            AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Other[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{GraphicSettings.SecondaryColor}]Additional Tools[/]")
                    .PageSize(GraphicSettings.PageSize)
                    .AddChoices(
                    [
                        "Fix Keyboard Issues",
                        "User Management",
                        "Run as Administrator",
                        "Back to Main Menu"
                    ]));

            switch (choice)
            {
                case "Fix Keyboard Issues":
                    Keyboard.FixKeyboard();
                    break;
                case "User Management":
                    Users.SettingsUser();
                    break;
                case "Run as Administrator":
                    RunAsAdmin();
                    break;
                case "Back to Main Menu":
                    Console.Clear();
                    return;
            }
        }
    }
    public class Keyboard
    {
        public static void FixKeyboard()
        {
            Console.Clear();

            if (!IsRunningAsAdmin())
            {
                ShowAdminWarning();
                return;
            }

            AnsiConsole.Write(
                new Panel("[bold red]WARNING: Keyboard Fix Utility[/]")
                    .BorderColor(GraphicSettings.GetThemeColor)
                    .Padding(1, 1));

            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]This tool attempts to fix common keyboard issues.[/]");
            AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Some operations require administrator privileges.[/]");

            if (!AnsiConsole.Confirm("\n[red]Continue with keyboard fix?[/]", false))
                return;

            try
            {
                var progress = AnsiConsole.Progress()
                    .Columns(
                    [
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new SpinnerColumn()
                    ]);

                progress.Start(ctx =>
                {
                    var task1 = ctx.AddTask($"[{GraphicSettings.SecondaryColor}]Checking keyboard status...[/]");
                    task1.Increment(25);

                    if (IsKeyboardWorking())
                    {
                        AnsiConsole.MarkupLine($"\n[{GraphicSettings.SecondaryColor}]✅ Keyboard appears to be working normally.[/]");
                        return;
                    }

                    task1.Increment(25);

                    var task2 = ctx.AddTask($"[{GraphicSettings.SecondaryColor}]Attempting driver reset...[/]");
                    ResetKeyboardDriver();
                    task2.Increment(100);

                    var task3 = ctx.AddTask($"[{GraphicSettings.SecondaryColor}]Checking registry settings...[/]");
                    EnableKeyboardInRegistry();
                    task3.Increment(100);

                    var task4 = ctx.AddTask($"[{GraphicSettings.SecondaryColor}]Restarting keyboard service...[/]");
                    RestartKeyboardService();
                    task4.Increment(100);

                    var task5 = ctx.AddTask($"[{GraphicSettings.SecondaryColor}]Enabling in Device Manager...[/]");
                    EnableInDeviceManager();
                    task5.Increment(100);
                });

                ShowResultPanel();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static bool IsKeyboardWorking()
        {
            AnsiConsole.Markup($"[{GraphicSettings.SecondaryColor}]Press any key within 5 seconds to test...[/]");

            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalSeconds < 5)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Key detected![/]");
                    return true;
                }
                Thread.Sleep(100);
            }

            AnsiConsole.MarkupLine("\n[red]No keypress detected.[/]");
            return false;
        }

        private static void ResetKeyboardDriver()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-Command \"Get-PnpDevice -Class Keyboard | Reset-PnpDevice -Confirm:$false\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                Process.Start(psi)?.WaitForExit(10000);
            }
            catch { }
        }

        private static void EnableKeyboardInRegistry()
        {
            string[] regCommands =
            [
                @"add HKLM\SYSTEM\CurrentControlSet\Services\i8042prt /v Start /t REG_DWORD /d 1 /f",
                @"add HKLM\SYSTEM\CurrentControlSet\Services\kbdclass /v Start /t REG_DWORD /d 1 /f",
                @"add HKLM\SYSTEM\CurrentControlSet\Services\kbdhid /v Start /t REG_DWORD /d 1 /f"
            ];

            foreach (string cmd in regCommands)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = cmd,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    })?.WaitForExit(3000);
                }
                catch { }
            }
        }

        private static void RestartKeyboardService()
        {
            string[] services = ["i8042prt", "kbdclass", "kbdhid"];

            foreach (string serviceName in services)
            {
                try
                {
                    using var sc = new ServiceController(serviceName);
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                    }
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(3));
                }
                catch { }
            }
        }

        private static void EnableInDeviceManager()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mmc.exe",
                    Arguments = "devmgmt.msc",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private static void ShowResultPanel()
        {
            var panel = new Panel(
                $"[{GraphicSettings.SecondaryColor}]KEYBOARD FIX COMPLETE[/]\n\n" +
                $"[{GraphicSettings.SecondaryColor}]Recommended actions:[/]\n" +
                $"1. [{GraphicSettings.SecondaryColor}]Test your keyboard now[/]\n" +
                $"2. [{GraphicSettings.SecondaryColor}]Restart computer if issues persist[/]\n" +
                $"3. [{GraphicSettings.SecondaryColor}]Check physical connections[/]\n" +
                $"4. [{GraphicSettings.SecondaryColor}]Try different USB port[/]\n" +
                $"5. [{GraphicSettings.SecondaryColor}]Update keyboard drivers[/]\n\n" +
                $"[{GraphicSettings.NeutralColor}]Note: Some fixes require system restart.[/]")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(GraphicSettings.GetThemeColor),
                Padding = new Padding(2, 1, 2, 1)
            };

            AnsiConsole.Write(panel);
        }
    }

    public class Users
    {
        public static void SettingsUser()
        {
            while (true)
            {
                Console.Clear();

                if (!IsRunningAsAdmin())
                {
                    ShowAdminWarning();
                    return;
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[{GraphicSettings.SecondaryColor}]👥 User Management[/]")
                        .PageSize(GraphicSettings.PageSize)
                        .AddChoices(
                        [
                            "List All Users",
                            "Create New User",
                            "Delete User",
                            "Change User Password",
                            "Add User to Administrators",
                            "User Information",
                            "Back"
                        ]));

                switch (choice)
                {
                    case "List All Users":
                        PrintAllUsers();
                        break;
                    case "Create New User":
                        CreateNewUser();
                        break;
                    case "Delete User":
                        DeleteUser();
                        break;
                    case "Change User Password":
                        ChangeUserPassword();
                        break;
                    case "Add User to Administrators":
                        AddUserToAdminGroup();
                        break;
                    case "User Information":
                        ShowUserInfo();
                        break;
                    case "Back":
                        return;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public static void PrintAllUsers()
        {
            try
            {
                AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Local Users[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

                using var ctx = new PrincipalContext(ContextType.Machine);
                using var qbeUser = new UserPrincipal(ctx);
                using var srch = new PrincipalSearcher(qbeUser);

                var table = new Table()
                    .Border(TableBorder.HeavyEdge)
                    .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Username[/]").LeftAligned())
                    .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Full Name[/]").LeftAligned())
                    .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Description[/]").LeftAligned())
                    .AddColumn(new TableColumn($"[{GraphicSettings.SecondaryColor}]Enabled[/]").Centered());

                foreach (UserPrincipal found in srch.FindAll().Cast<UserPrincipal>())
                {
                    table.AddRow(
                        $"[{GraphicSettings.SecondaryColor}]{found.SamAccountName}[/]",
                        $"[{GraphicSettings.SecondaryColor}]{found.DisplayName ?? "N/A"}[/]",
                        $"[{GraphicSettings.SecondaryColor}]{found.Description ?? "N/A"}[/]",
                        found.Enabled == true ? $"[{GraphicSettings.SecondaryColor}]Yes[/]" : "[red]No[/]");
                }

                AnsiConsole.Write(table);

                // Также показываем группы
                ShowLocalGroups();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        public static void CreateNewUser()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[{GraphicSettings.AccentColor}]CREATE LOCAL USER[/]").RuleStyle(GraphicSettings.SecondaryColor).LeftJustified());

            var username = AnsiConsole.Prompt(
                new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter username:[/]")
                    .PromptStyle(GraphicSettings.AccentColor)
                    .Validate(name =>
                        !string.IsNullOrWhiteSpace(name) && name.Length >= 3
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Username must be at least 3 characters[/]")));

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter password:[/]")
                    .PromptStyle(GraphicSettings.AccentColor)
                    .Secret()
                    .Validate(pass =>
                        pass.Length >= 6
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Password must be at least 6 characters[/]")));

            var fullName = AnsiConsole.Prompt(
                new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter full name (optional):[/]")
                    .PromptStyle(GraphicSettings.AccentColor)
                    .AllowEmpty());

            var description = AnsiConsole.Prompt(
                new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter description (optional):[/]")
                    .PromptStyle(GraphicSettings.AccentColor)
                    .AllowEmpty());

            try
            {
                using var ctx = new PrincipalContext(ContextType.Machine);

                if (UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username) != null)
                {
                    AnsiConsole.MarkupLine($"[yellow]User '{username}' already exists![/]");
                    return;
                }

                using var user = new UserPrincipal(ctx)
                {
                    SamAccountName = username,
                    Name = fullName,
                    DisplayName = fullName,
                    Description = description,
                    PasswordNeverExpires = true,
                    Enabled = true,
                    UserCannotChangePassword = false
                };

                user.SetPassword(password);
                user.Save();

                AnsiConsole.MarkupLine($"[{GraphicSettings.AccentColor}] User '{username}' created successfully![/]");

                AnsiConsole.MarkupLine($"[{GraphicSettings.NeutralColor}]Press any key to continue...[/]");
                Console.ReadKey();
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Access denied! Run as Administrator.[/]");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error creating user: {ex.Message}[/]");
                Console.ReadKey();
            }
        }
        public static void DeleteUser()
        {
            try
            {

                using var ctx = new PrincipalContext(ContextType.Machine);

                using var qbeUser = new UserPrincipal(ctx);

                using var srch = new PrincipalSearcher(qbeUser);



                var users = srch.FindAll()
                    .Cast<UserPrincipal>()
                    .Select(u => u.SamAccountName)
                    .Where(u => !u.Equals("Administrator", StringComparison.OrdinalIgnoreCase) &&
                               !u.Equals("Guest", StringComparison.OrdinalIgnoreCase))
                    .ToList();


                if (users.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]No users found to delete.[/]");
                    return;
                }

                var userToDelete = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[{GraphicSettings.SecondaryColor}]Select user to delete:[/]")
                        .PageSize(GraphicSettings.PageSize)
                        .AddChoices(users));

                if (!AnsiConsole.Confirm($"[bold red]Are you sure you want to delete user '{userToDelete}'?[/]", false))
                    return;


                using var user = UserPrincipal.FindByIdentity(ctx, userToDelete);

                if (user != null)
                {

                    user.Delete();

                    AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]User '{userToDelete}' deleted successfully![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error deleting user: {ex.Message}[/]");
            }
        }

        public static void ChangeUserPassword()
        {
            try
            {

                using var ctx = new PrincipalContext(ContextType.Machine);

                var users = GetUserList(ctx);

                var username = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select user to change password:")
                        .AddChoices(users));

                var newPassword = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[{GraphicSettings.SecondaryColor}]Enter new password:[/]")
                        .PromptStyle(GraphicSettings.AccentColor)
                        .Secret()
                        .Validate(pass =>
                            pass.Length >= 6
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Password must be at least 6 characters[/]")));


                using var user = UserPrincipal.FindByIdentity(ctx, username);

                if (user != null)
                {

                    user.SetPassword(newPassword);

                    user.Save();

                    AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]Password changed for user '{username}'![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        public static void AddUserToAdminGroup()
        {
            try
            {

                using var ctx = new PrincipalContext(ContextType.Machine);

                var users = GetUserList(ctx);

                var username = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select user to add to Administrators:")
                        .AddChoices(users));


                using var user = UserPrincipal.FindByIdentity(ctx, username);

                using var group = GroupPrincipal.FindByIdentity(ctx, "Administrators");


                if (user != null && group != null)
                {

                    if (!group.Members.Contains(user))
                    {

                        group.Members.Add(user);

                        group.Save();

                        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]User '{username}' added to Administrators group![/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[{GraphicSettings.SecondaryColor}]User '{username}' is already in Administrators group.[/]");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        public static void ShowUserInfo()
        {
            try
            {

                using var ctx = new PrincipalContext(ContextType.Machine);

                var users = GetUserList(ctx);

                var username = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[{GraphicSettings.SecondaryColor}]Select user for details:[/]")
                        .AddChoices(users));


                using var user = UserPrincipal.FindByIdentity(ctx, username);

                if (user != null)
                {

                    var panel = new Panel(
                        $"[{GraphicSettings.SecondaryColor}]User Information: {user.SamAccountName}[/]\n\n" +
                        $"[{GraphicSettings.SecondaryColor}]Full Name:[/] [white]{user.DisplayName ?? "N/A"}[/]\n" +
                        $"[{GraphicSettings.SecondaryColor}]Description:[/] [white]{user.Description ?? "N/A"}[/]\n" +
                        $"[{GraphicSettings.SecondaryColor}]Email:[/] [white]{user.EmailAddress ?? "N/A"}[/]\n" +
                        $"[{GraphicSettings.SecondaryColor}]Enabled:[/] {(user.Enabled == true ? $"[{GraphicSettings.AccentColor}]Yes[/]" : "[red]No[/]")}\n" +
                        $"[{GraphicSettings.SecondaryColor}]Password Never Expires:[/] {(user.PasswordNeverExpires ? $"[{GraphicSettings.SecondaryColor}]Yes[/]" : "[red]No[/]")}\n" +
                        $"[{GraphicSettings.SecondaryColor}]Last Logon:[/] [white]{user.LastLogon?.ToString("yyyy-MM-dd HH:mm") ?? "Never"}[/]\n" +
                        $"[{GraphicSettings.SecondaryColor}]Account Created:[/] [white]{user.Context.ConnectedServer ?? "N/A"}[/]")
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(GraphicSettings.GetThemeColor), //dodelat'
                        Padding = new Padding(1, 1, 1, 1)
                    };


                    AnsiConsole.Write(panel);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        private static List<string> GetUserList(PrincipalContext ctx)
        {

            using var qbeUser = new UserPrincipal(ctx);

            using var srch = new PrincipalSearcher(qbeUser);

            return [.. srch.FindAll()
                .Cast<UserPrincipal>()
                .Select(u => u.SamAccountName)
                .Where(u => !u.Equals("Administrator", StringComparison.OrdinalIgnoreCase) &&
                           !u.Equals("Guest", StringComparison.OrdinalIgnoreCase))];


        }

        private static void ShowLocalGroups()
        {
            try
            {
                AnsiConsole.Write(new Rule($"[{GraphicSettings.SecondaryColor}]Local Groups[/]").RuleStyle(GraphicSettings.AccentColor).LeftJustified());

                using var ctx = new PrincipalContext(ContextType.Machine);

                using var qbeGroup = new GroupPrincipal(ctx);

                using var srch = new PrincipalSearcher(qbeGroup);

                var groups = srch.FindAll()
                    .Cast<GroupPrincipal>()
                    .Select(g => g.Name)
                    .OrderBy(g => g)
                    .ToList();

                var grid = new Grid()
                    .AddColumn(new GridColumn().PadRight(2))
                    .AddColumn(new GridColumn().PadRight(2))
                    .AddColumn(new GridColumn());

                for (int i = 0; i < groups.Count; i += 3)
                {
                    var row = new List<string>();
                    for (int j = 0; j < 3 && i + j < groups.Count; j++)
                    {
                        row.Add($"[{GraphicSettings.SecondaryColor}]•[/] {groups[i + j]}");
                    }
                    grid.AddRow(row.ToArray());
                }

                AnsiConsole.Write(grid);
            }
            catch { }
        }
    }

    protected static bool IsRunningAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);

        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    protected static void ShowAdminWarning()
    {
        var panel = new Panel(
            $"[red]ADMINISTRATOR RIGHTS REQUIRED[/]\n\n" +
            $"[{GraphicSettings.SecondaryColor}]This feature requires administrator privileges.[/]\n" +
            $"[{GraphicSettings.SecondaryColor}]Please run the program as Administrator.[/]\n\n" +
            $"[{GraphicSettings.SecondaryColor}]To run as Administrator:[/]\n" +
            $"1. Right-click the program\n" +
            $"2. Select 'Run as administrator'\n" +
            $"3. Confirm UAC prompt")
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(GraphicSettings.GetThemeColor), //dodelat'
            Padding = new Padding(2, 1, 2, 1)
        };

        AnsiConsole.Write(panel);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void RunAsAdmin()
    {
        try
        {
            var exePath = Environment.ProcessPath;
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Verb = "runas",
                UseShellExecute = true
            });

            Environment.Exit(0);
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Failed to restart as administrator.[/]");
        }
    }
}
