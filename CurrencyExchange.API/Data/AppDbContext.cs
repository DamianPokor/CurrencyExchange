using Microsoft.EntityFrameworkCore;
using CurrencyExchange.API.Data.Entities;


//Dodac identity user! 

namespace CurrencyExchange.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountId);

                entity.Property(a => a.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(26);

                entity.Property(a => a.Balance)
                      .HasColumnType("decimal(18,2)");

                entity.Property(a => a.Currency)
                      .IsRequired()
                      .HasMaxLength(3);

                entity.Property(a => a.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.CurrencyNavigation)
                      .WithMany(c => c.Accounts)
                      .HasForeignKey(a => a.Currency)
                      .HasPrincipalKey(c => c.Code)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Currency configuration
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Code);

                entity.Property(c => c.Code)
                      .HasMaxLength(3);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.eMail)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
