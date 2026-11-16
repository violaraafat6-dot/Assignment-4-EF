using NationalBankManagementSystem.Models.Enums;
using NationalBankManagementSystem.Services;

namespace NationalBankManagementSystem.Services;

public class InteractiveMenu
{
    private readonly BankService _bankService;

    public InteractiveMenu(BankService bankService)
    {
        _bankService = bankService;
    }

    public void Run()
    {
        bool exit = false;

        while (!exit)
        {
            if (!Console.IsOutputRedirected)
            {
                try { Console.Clear(); } catch { }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("=========================================");
            Console.WriteLine("  National Bank - Management");
            Console.WriteLine("=========================================");
            Console.WriteLine("  1) Add a new Customer");
            Console.WriteLine("  2) Open a new Account for a Customer");
            Console.WriteLine("  3) Update Account Status (Active / Closed)");
            Console.WriteLine("  4) Remove an Account from a Customer");
            Console.WriteLine("  5) List all Customers (with accounts)");
            Console.WriteLine("  0) Exit");
            Console.WriteLine("-----------------------------------------");
            Console.Write("Enter choice: ");

            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                ShowError("Invalid choice! Please enter a valid number (0 - 5).");
                Pause();
                continue;
            }

            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    HandleAddCustomer();
                    break;
                case 2:
                    HandleOpenAccount();
                    break;
                case 3:
                    HandleUpdateAccountStatus();
                    break;
                case 4:
                    HandleRemoveAccount();
                    break;
                case 5:
                    HandleListCustomers();
                    break;
                case 0:
                    exit = true;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Exiting application and closing connection. Goodbye!");
                    Console.ResetColor();
                    break;
                default:
                    ShowError("Unknown choice! Please select an option between 0 and 5.");
                    Pause();
                    break;
            }
        }
    }

    private void HandleAddCustomer()
    {
        Console.WriteLine("--- Add New Customer ---");

        string fullName = ReadRequiredString("Full Name     : ");
        string nationalId = ReadRequiredString("National ID   : ");
        DateTime dob = ReadDate("Date of Birth : (yyyy-MM-dd) ");
        string email = ReadRequiredString("Email         : ");
        string phone = ReadRequiredString("Phone         : ");
        string address = ReadRequiredString("Address       : ");

        Console.WriteLine("Customer Type:");
        Console.WriteLine("  1) Individual");
        Console.WriteLine("  2) Business");
        int typeChoice = ReadIntInRange("Choice: ", 1, 2);
        CustomerType customerType = (CustomerType)typeChoice;

        var customer = _bankService.AddCustomer(fullName, nationalId, dob, email, phone, address, customerType);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
        Console.ResetColor();

        Pause();
    }

    private void HandleOpenAccount()
    {
        Console.WriteLine("--- Open New Account ---");

        string accountNumber = ReadRequiredString("Account Number : ");

        Console.WriteLine("Account Type:");
        Console.WriteLine("  1) Savings");
        Console.WriteLine("  2) Current");
        Console.WriteLine("  3) Business");
        int typeChoice = ReadIntInRange("Choice: ", 1, 3);
        AccountType accountType = (AccountType)typeChoice;

        string branchCode = ReadRequiredString("Branch Code    : ");
        int customerId = ReadInt("Customer Id    : ");

        Console.WriteLine("Ownership Role:");
        Console.WriteLine("  1) Primary");
        Console.WriteLine("  2) CoHolder");
        int roleChoice = ReadIntInRange("Choice: ", 1, 2);
        OwnershipRole role = (OwnershipRole)roleChoice;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");
        Console.ResetColor();

        var result = _bankService.OpenAccount(accountNumber, accountType, branchCode, customerId, role);

        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result.Message);
            Console.ResetColor();
        }
        else
        {
            ShowError(result.Message);
        }

        Pause();
    }

    private void HandleUpdateAccountStatus()
    {
        Console.WriteLine("--- Update Account Status ---");

        string accountNumber = ReadRequiredString("Account Number : ");
        int customerId = ReadInt("Customer Id    : ");

        Console.WriteLine("New Status:");
        Console.WriteLine("  1) Active");
        Console.WriteLine("  2) Closed");
        int statusChoice = ReadIntInRange("Choice: ", 1, 2);
        AccountStatus newStatus = (AccountStatus)statusChoice;

        var result = _bankService.UpdateAccountStatus(accountNumber, customerId, newStatus);

        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result.Message);
            Console.ResetColor();
        }
        else
        {
            ShowError(result.Message);
        }

        Pause();
    }

    private void HandleRemoveAccount()
    {
        Console.WriteLine("--- Remove Account From Customer ---");

        string accountNumber = ReadRequiredString("Account Number : ");
        int customerId = ReadInt("Customer Id    : ");
        Console.WriteLine();

        var result = _bankService.RemoveAccountFromCustomer(accountNumber, customerId);

        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result.Message);
            Console.ResetColor();

            if (result.AccountDeleted)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"That was the last owner - account '{accountNumber}' was also removed.");
                Console.ResetColor();
            }
        }
        else
        {
            ShowError(result.Message);
        }

        Pause();
    }

    private void HandleListCustomers()
    {
        Console.WriteLine("--- All Customers ---");
        Console.WriteLine();

        var customers = _bankService.GetAllCustomersWithAccounts();

        if (customers.Count == 0)
        {
            Console.WriteLine("(no customers found in system)");
        }
        else
        {
            foreach (var customer in customers)
            {
                Console.WriteLine($"#{customer.Id} {customer.FullName} ({customer.CustomerType})");

                if (customer.CustomerAccounts == null || customer.CustomerAccounts.Count == 0)
                {
                    Console.WriteLine("    (no accounts)");
                }
                else
                {
                    foreach (var ca in customer.CustomerAccounts)
                    {
                        var acc = ca.Account;
                        string accNum = acc?.AccountNumber ?? "N/A";
                        string accType = acc?.AccountType.ToString() ?? "Unknown";
                        decimal balance = acc?.CurrentBalance ?? 0m;
                        string role = ca.OwnershipType.ToString();
                        string status = ca.AccountStatus.ToString();
                        string branchName = acc?.Branch?.Name ?? "Unknown Branch";

                        Console.WriteLine($"    {accNum,-9} {accType,-9} Balance: {balance,10:N2}  {role,-7}  {status,-6}  @ {branchName}");
                    }
                }
            }
        }

        Pause();
    }

    #region Input Helpers

    private static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            ShowError("This field cannot be empty. Please try again.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value))
            {
                return value;
            }
            ShowError("Invalid number format. Please enter an integer.");
        }
    }

    private static int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }
            ShowError($"Invalid option. Please choose a number between {min} and {max}.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (DateTime.TryParse(input, out DateTime date))
            {
                return date;
            }
            ShowError("Invalid date format. Please enter date as (yyyy-MM-dd), e.g. 2000-01-25.");
        }
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to return to the menu...");
        Console.ResetColor();
        if (!Console.IsInputRedirected)
        {
            try { Console.ReadKey(intercept: true); } catch { }
        }
        else
        {
            Console.ReadLine();
        }
    }

    #endregion
}
