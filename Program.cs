using NationalBankManagementSystem.Data;
using NationalBankManagementSystem.Services;

namespace NationalBankManagementSystem;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (!Console.IsOutputRedirected)
            {
                try { Console.Title = "National Bank Management System"; } catch { }
            }
            using var context = new AppDbContext();

            Console.WriteLine("Connecting to SQL Server and applying migrations...");
            DbInitializer.Initialize(context);
            Console.WriteLine("Database ready and seeded successfully!");

            var bankService = new BankService(context);
            var menu = new InteractiveMenu(bankService);
            menu.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Critical Error] {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            if (!Console.IsInputRedirected)
            {
                try { Console.ReadKey(); } catch { }
            }
        }
    }
}
