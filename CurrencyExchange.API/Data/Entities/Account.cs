using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.API.Data.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        
        public int UserId { get; set; }
        
        public string AccountNumber { get; set; } = string.Empty;
        
        public decimal Balance { get; set; }
        
        public string Currency { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nawigacje 
        public virtual User? User { get; set; }
        public virtual Currency? CurrencyNavigation { get; set; }
    }
}

