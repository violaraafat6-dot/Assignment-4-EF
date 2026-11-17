using Microsoft.EntityFrameworkCore;

namespace NationalBankManagementSystem;

public class BankService
{
    private readonly AppDbContext _context;

    public BankService(AppDbContext context)
    {
        _context = context;
    }

    // 1) Add a new Customer
    public Customer AddCustomer(string fullName, string nationalId, DateTime dateOfBirth, 
                                string email, string phone, string address, CustomerType customerType)
    {
        var customer = new Customer
        {
            FullName = fullName.Trim(),
            NationalId = nationalId.Trim(),
            DateOfBirth = dateOfBirth,
            Email = email.Trim(),
            PhoneNumber = phone.Trim(),
            Address = address.Trim(),
            CustomerType = customerType
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();
        return customer;
    }

    // 2) Open a new Account for a Customer
    public (bool Success, string Message) OpenAccount(string accountNumber, AccountType accountType, 
                                                      string branchCode, int customerId, OwnershipRole role)
    {
        // Validate Branch
        var branch = _context.Branches.FirstOrDefault(b => b.Code.ToUpper() == branchCode.Trim().ToUpper());
        if (branch == null)
        {
            return (false, $"Error: Branch with code '{branchCode}' does not exist.");
        }

        // Validate Customer
        var customer = _context.Customers.Find(customerId);
        if (customer == null)
        {
            return (false, $"Error: Customer with ID #{customerId} does not exist.");
        }

        // Check if Account exists
        var existingAccount = _context.Accounts.Find(accountNumber.Trim());
        if (existingAccount == null)
        {
            // Create new Account
            existingAccount = new Account
            {
                AccountNumber = accountNumber.Trim(),
                AccountType = accountType,
                CurrentBalance = 0.00m,
                OpeningDate = DateTime.Now,
                BranchCode = branch.Code
            };
            _context.Accounts.Add(existingAccount);
        }
        else
        {
            // Verify if customer already has this account
            var alreadyLinked = _context.CustomerAccounts
                .Any(ca => ca.CustomerId == customerId && ca.AccountNumber == accountNumber.Trim());
            if (alreadyLinked)
            {
                return (false, $"Customer #{customerId} is already linked to account '{accountNumber}'.");
            }
        }

        // Create CustomerAccount link
        var customerAccount = new CustomerAccount
        {
            CustomerId = customerId,
            AccountNumber = accountNumber.Trim(),
            OwnershipType = role,
            OwnershipStartDate = DateTime.Now,
            AccountStatus = AccountStatus.Active
        };

        _context.CustomerAccounts.Add(customerAccount);
        _context.SaveChanges();

        return (true, $"Account '{accountNumber}' created and linked to customer {customerId} as {role} owner.");
    }

    // 3) Update Account Status
    public (bool Success, string Message) UpdateAccountStatus(string accountNumber, int customerId, AccountStatus newStatus)
    {
        var customerAccount = _context.CustomerAccounts
            .FirstOrDefault(ca => ca.AccountNumber == accountNumber.Trim() && ca.CustomerId == customerId);

        if (customerAccount == null)
        {
            return (false, $"No account '{accountNumber}' found for customer ID #{customerId}.");
        }

        customerAccount.AccountStatus = newStatus;
        _context.SaveChanges();

        return (true, $"Status updated to {newStatus}.");
    }

    // 4) Remove an Account from a Customer
    public (bool Success, bool AccountDeleted, string Message) RemoveAccountFromCustomer(string accountNumber, int customerId)
    {
        var customerAccount = _context.CustomerAccounts
            .FirstOrDefault(ca => ca.AccountNumber == accountNumber.Trim() && ca.CustomerId == customerId);

        if (customerAccount == null)
        {
            return (false, false, $"No record found for account '{accountNumber}' linked to customer ID #{customerId}.");
        }

        _context.CustomerAccounts.Remove(customerAccount);
        _context.SaveChanges();

        // Check if this was the last owner
        bool hasOtherOwners = _context.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber.Trim());
        bool accountDeleted = false;

        if (!hasOtherOwners)
        {
            var account = _context.Accounts.Find(accountNumber.Trim());
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                accountDeleted = true;
            }
        }

        return (true, accountDeleted, "Ownership link deleted.");
    }

    // 5) List all Customers (with accounts) - Eager Loading with Include & ThenInclude
    public List<Customer> GetAllCustomersWithAccounts()
    {
        return _context.Customers
            .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
                    .ThenInclude(a => a.Branch)
            .OrderBy(c => c.Id)
            .ToList();
    }
}
