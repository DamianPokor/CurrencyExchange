using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.API.Data.Entities;

namespace CurrencyExchange.API.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Nie dodajemy DbSet<User> — jest w IdentityDbContext

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // konieczne dla Identity!

            // Konfiguracja User (własne pola rozszerzające IdentityUser)
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // Konfiguracja Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountId);

                entity.Property(a => a.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(26);

                entity.Property(a => a.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

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

            // Konfiguracja Currency
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Code);

                entity.Property(c => c.Code)
                      .HasMaxLength(3);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // Konfiguracja Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity.Property(t => t.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(t => t.ExchangeRate)
                      .HasColumnType("decimal(18,6)");

                entity.Property(t => t.TransactionDate)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne<Account>()
                      .WithMany()
                      .HasForeignKey(t => t.FromAccID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Account>()
                      .WithMany()
                      .HasForeignKey(t => t.ToAccID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

/*
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.API.Data.Entities;



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
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountId);

                entity.Property(a => a.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(26);

                entity.Property(a => a.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

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

            
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Code);

                entity.Property(c => c.Code)
                      .HasMaxLength(3);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            
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
            
            modelBuilder.Entity<Transaction>(entity =>
            {
                  entity.HasKey(t => t.TransactionId);

                  entity.Property(t => t.Amount)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                  entity.Property(t => t.ExchangeRate)
                        .HasColumnType("decimal(18,6)");

                  entity.Property(t => t.TransactionDate)
                        .HasDefaultValueSql("GETUTCDATE()");

                  // Relacje do kont
                  entity.HasOne<Account>()
                        .WithMany()
                        .HasForeignKey(t => t.FromAccID)
                        .OnDelete(DeleteBehavior.Restrict);

                  entity.HasOne<Account>()
                        .WithMany()
                        .HasForeignKey(t => t.ToAccID)
                        .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
*/
