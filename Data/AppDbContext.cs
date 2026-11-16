using Microsoft.EntityFrameworkCore;
using NationalBankManagementSystem.Models;

namespace NationalBankManagementSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Branch Configuration
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(b => b.Code);
            entity.Property(b => b.Code).HasMaxLength(20);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(150);
            entity.Property(b => b.Address).HasMaxLength(250);
            entity.Property(b => b.PhoneNumber).HasMaxLength(30);

            // 1-to-1 with Manager (Branch has ManagerId FK)
            entity.HasOne(b => b.Manager)
                  .WithOne(m => m.Branch)
                  .HasForeignKey<Branch>(b => b.ManagerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Manager Configuration
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.FullName).IsRequired().HasMaxLength(150);
            entity.Property(m => m.Email).HasMaxLength(100);
            entity.Property(m => m.PhoneNumber).HasMaxLength(30);
            entity.Property(m => m.HireDate).HasColumnType("datetime2");
        });

        // Customer Configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.NationalId).IsRequired().HasMaxLength(30);
            entity.Property(c => c.Email).HasMaxLength(100);
            entity.Property(c => c.PhoneNumber).HasMaxLength(30);
            entity.Property(c => c.Address).HasMaxLength(250);
            entity.Property(c => c.DateOfBirth).HasColumnType("date");
            entity.Property(c => c.CustomerType).HasConversion<string>().HasMaxLength(20);
        });

        // Account Configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.AccountNumber);
            entity.Property(a => a.AccountNumber).HasMaxLength(50);
            entity.Property(a => a.AccountType).HasConversion<string>().HasMaxLength(30);
            entity.Property(a => a.CurrentBalance).HasColumnType("decimal(18,2)");
            entity.Property(a => a.OpeningDate).HasColumnType("datetime2");

            // 1-to-Many: Branch has many Accounts
            entity.HasOne(a => a.Branch)
                  .WithMany(b => b.Accounts)
                  .HasForeignKey(a => a.BranchCode)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CustomerAccount Configuration (Many-to-Many Join Table: "Have")
        modelBuilder.Entity<CustomerAccount>(entity =>
        {
            entity.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

            entity.Property(ca => ca.OwnershipType).HasConversion<string>().HasMaxLength(30);
            entity.Property(ca => ca.AccountStatus).HasConversion<string>().HasMaxLength(30);
            entity.Property(ca => ca.OwnershipStartDate).HasColumnType("datetime2");

            entity.HasOne(ca => ca.Customer)
                  .WithMany(c => c.CustomerAccounts)
                  .HasForeignKey(ca => ca.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ca => ca.Account)
                  .WithMany(a => a.CustomerAccounts)
                  .HasForeignKey(ca => ca.AccountNumber)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction Configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.TransactionNumber);
            entity.Property(t => t.TransactionNumber).ValueGeneratedOnAdd();
            entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(30);
            entity.Property(t => t.TransactionDate).HasColumnType("datetime2");
            entity.Property(t => t.Note).HasMaxLength(500);

            // 1-to-Many: Account has many Transactions
            entity.HasOne(t => t.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(t => t.AccountNumber)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
