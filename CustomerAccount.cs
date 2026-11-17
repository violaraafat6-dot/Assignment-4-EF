namespace NationalBankManagementSystem;

public class CustomerAccount
{
    // Composite Key
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string AccountNumber { get; set; } = string.Empty;
    public Account Account { get; set; } = null!;

    // Relationship attributes defined in ER diagram
    public OwnershipRole OwnershipType { get; set; }
    public DateTime OwnershipStartDate { get; set; }
    public AccountStatus AccountStatus { get; set; }
}
