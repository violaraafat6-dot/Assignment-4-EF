namespace NationalBankManagementSystem.Models;

public class Branch
{
    // Primary Key (e.g. "CAI-01", "ALX-01")
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // 1-to-1 Relationship with Manager (Manage)
    public int? ManagerId { get; set; }
    public Manager? Manager { get; set; }

    // 1-to-Many Relationship with Account (Belong)
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
