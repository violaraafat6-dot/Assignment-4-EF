namespace NationalBankManagementSystem;

public class Transaction
{
    // Primary Key
    public int TransactionNumber { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Note { get; set; }

    // Foreign Key to Account (Related)
    public string AccountNumber { get; set; } = string.Empty;
    public Account Account { get; set; } = null!;
}
