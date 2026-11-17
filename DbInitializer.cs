using Microsoft.EntityFrameworkCore;

namespace NationalBankManagementSystem;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Ensure database is created and migrations applied
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Migration Notice] {ex.Message}. Falling back to EnsureCreated...");
            context.Database.EnsureCreated();
        }

        // Check if data already exists
        if (context.Branches.Any() || context.Managers.Any())
        {
            return; // DB has already been seeded
        }

        // 1. Seed Managers
        var managerCairo = new Manager
        {
            FullName = "Hassan Mostafa",
            Email = "hassan.mostafa@nationalbank.com",
            PhoneNumber = "01001234567",
            HireDate = new DateTime(2018, 5, 1)
        };

        var managerAlex = new Manager
        {
            FullName = "Noha El-Sayed",
            Email = "noha.elsayed@nationalbank.com",
            PhoneNumber = "01112345678",
            HireDate = new DateTime(2020, 2, 15)
        };

        context.Managers.AddRange(managerCairo, managerAlex);
        context.SaveChanges();

        // 2. Seed Branches (Part 02 Requirement: Seed at least Branch and Manager)
        var branchCairo = new Branch
        {
            Code = "CAI-01",
            Name = "Cairo Main Branch",
            Address = "15 Tahrir Sq, Downtown, Cairo",
            PhoneNumber = "02-25789000",
            ManagerId = managerCairo.Id
        };

        var branchAlex = new Branch
        {
            Code = "ALX-01",
            Name = "Alexandria Branch",
            Address = "22 El-Geish Rd, Corniche, Alexandria",
            PhoneNumber = "03-4856000",
            ManagerId = managerAlex.Id
        };

        context.Branches.AddRange(branchCairo, branchAlex);
        context.SaveChanges();

        // 3. Seed initial Customers
        var customer1 = new Customer
        {
            FullName = "Ahmed Ali",
            NationalId = "29501011234567",
            DateOfBirth = new DateTime(1995, 1, 1),
            Email = "ahmed.ali@example.com",
            PhoneNumber = "01011223344",
            Address = "Nasr City, Cairo",
            CustomerType = CustomerType.Individual
        };

        var customer2 = new Customer
        {
            FullName = "Nile Trading LLC",
            NationalId = "10020030040050",
            DateOfBirth = new DateTime(2010, 6, 15),
            Email = "info@niletrading.com",
            PhoneNumber = "01233445566",
            Address = "Smouha, Alexandria",
            CustomerType = CustomerType.Business
        };

        context.Customers.AddRange(customer1, customer2);
        context.SaveChanges();

        // 4. Seed initial Accounts
        var account1 = new Account
        {
            AccountNumber = "2001-CUR",
            AccountType = AccountType.Current,
            CurrentBalance = 15400.00m,
            OpeningDate = new DateTime(2023, 1, 10),
            BranchCode = "CAI-01"
        };

        var account2 = new Account
        {
            AccountNumber = "3000-BUS",
            AccountType = AccountType.Business,
            CurrentBalance = 120000.00m,
            OpeningDate = new DateTime(2023, 3, 20),
            BranchCode = "ALX-01"
        };

        context.Accounts.AddRange(account1, account2);
        context.SaveChanges();

        // 5. Seed CustomerAccounts (Have relationship)
        var customerAccount1 = new CustomerAccount
        {
            CustomerId = customer1.Id,
            AccountNumber = account1.AccountNumber,
            OwnershipType = OwnershipRole.Primary,
            OwnershipStartDate = new DateTime(2023, 1, 10),
            AccountStatus = AccountStatus.Active
        };

        var customerAccount2 = new CustomerAccount
        {
            CustomerId = customer2.Id,
            AccountNumber = account2.AccountNumber,
            OwnershipType = OwnershipRole.Primary,
            OwnershipStartDate = new DateTime(2023, 3, 20),
            AccountStatus = AccountStatus.Active
        };

        context.CustomerAccounts.AddRange(customerAccount1, customerAccount2);
        context.SaveChanges();
    }
}
