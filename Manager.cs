namespace NationalBankManagementSystem;

public class Manager
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }

    // 1-to-1 Navigation property with Branch
    public Branch? Branch { get; set; }
}
