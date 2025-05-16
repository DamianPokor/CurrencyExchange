using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.API.Data.Entities
{
    public class Currency
    {
        public string Code { get; set; } = string.Empty; // np. "USD", "EUR"
        public string Name { get; set; } = string.Empty;

        // Nawigacja
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
/*public class Currency
{
    public required string CurrencyCode { get; set; }
    public required string Name { get; set; }
}

modelBuilder.Entity<Currency>()
    .HasKey(c => c.CurrencyCode);//wskazujemy klucz główny
modelBuilder.Entity<Currency>()
    .Property(c => c.CurrencyCode)//wskazujemy o co nam chodzi
    .HasMaxLength(3);//ustawiamy maksymalną długość
        
modelBuilder.Entity<Currency>()
    .Property(c => c.Name)
    .HasMaxLength(255);//ustawiamy maksymalną długość*/