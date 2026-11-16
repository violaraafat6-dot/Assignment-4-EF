using NationalBankManagementSystem.Models.Enums;

namespace NationalBankManagementSystem.Models;

public class Account
{
    // Primary Key (e.g. "123456", "2001-CUR", "3000-BUS")
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime OpeningDate { get; set; }

    // Foreign Key to Branch (Belong)
    public string BranchCode { get; set; } = string.Empty;
    public Branch Branch { get; set; } = null!;

    // Many-to-Many Relationship with Customer through CustomerAccount (Have)
    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();

    // 1-to-Many Relationship with Transaction (Related)
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
