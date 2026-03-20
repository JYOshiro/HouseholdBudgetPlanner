using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HouseholdBudgetApi.Data;

/// <summary>
/// Entity Framework Core database context for the Household Budget API.
/// </summary>
public class ApplicationDbContext : DbContext
{
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter =
        new(
            v => NormalizeToUtc(v),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableDateTimeConverter =
        new(
            v => v.HasValue ? NormalizeToUtc(v.Value) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for users.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// DbSet for households.
    /// </summary>
    public DbSet<Household> Households { get; set; } = null!;

    /// <summary>
    /// DbSet for categories.
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>
    /// DbSet for expenses.
    /// </summary>
    public DbSet<Expense> Expenses { get; set; } = null!;

    /// <summary>
    /// DbSet for income entries.
    /// </summary>
    public DbSet<Income> IncomeEntries { get; set; } = null!;

    /// <summary>
    /// DbSet for budgets.
    /// </summary>
    public DbSet<Budget> Budgets { get; set; } = null!;

    /// <summary>
    /// DbSet for bills.
    /// </summary>
    public DbSet<Bill> Bills { get; set; } = null!;

    /// <summary>
    /// DbSet for savings goals.
    /// </summary>
    public DbSet<SavingsGoal> SavingsGoals { get; set; } = null!;

    /// <summary>
    /// DbSet for goal contributions.
    /// </summary>
    public DbSet<GoalContribution> GoalContributions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key to Household
            entity.HasOne(e => e.Household)
                .WithMany(h => h.Users)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create unique index on Email
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Household entity
        modelBuilder.Entity<Household>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CurrencySymbol).HasDefaultValue("$").HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsSystemDefault).HasDefaultValue(false);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key to Household (nullable for system categories)
            entity.HasOne(e => e.Household)
                .WithMany(h => h.Categories)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(19, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsShared).HasDefaultValue(false);
            entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign keys
            entity.HasOne(e => e.Household)
                .WithMany(h => h.Expenses)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PaidByUser)
                .WithMany()
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Income entity
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(19, 2);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign keys
            entity.HasOne(e => e.Household)
                .WithMany(h => h.IncomeEntries)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.IncomeEntries)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Budget entity
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(19, 2);
            entity.Property(e => e.Month).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign keys
            entity.HasOne(e => e.Household)
                .WithMany(h => h.Budgets)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one budget per household per category per month
            entity.HasIndex(e => new { e.HouseholdId, e.CategoryId, e.Month })
                .IsUnique()
                .HasDatabaseName("IX_Budget_Unique_HouseholdCategoryMonth");
        });

        // Configure Bill entity
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(19, 2);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsPaid).HasDefaultValue(false);
            entity.Property(e => e.DueDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign keys
            entity.HasOne(e => e.Household)
                .WithMany(h => h.Bills)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Bills)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure SavingsGoal entity
        modelBuilder.Entity<SavingsGoal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TargetAmount).HasPrecision(19, 2);
            entity.Property(e => e.CurrentAmount).HasPrecision(19, 2).HasDefaultValue(0);
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.CompletedDate).IsRequired(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key
            entity.HasOne(e => e.Household)
                .WithMany(h => h.SavingsGoals)
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GoalContribution entity
        modelBuilder.Entity<GoalContribution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(19, 2);
            entity.Property(e => e.ContributionDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign keys
            entity.HasOne(e => e.Goal)
                .WithMany(g => g.Contributions)
                .HasForeignKey(e => e.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ContributedByUser)
                .WithMany()
                .HasForeignKey(e => e.ContributedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ApplyUtcDateTimeConverters(modelBuilder);
    }

    private static void ApplyUtcDateTimeConverters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(UtcDateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcNullableDateTimeConverter);
                }
            }
        }
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value,
        };
    }
}
