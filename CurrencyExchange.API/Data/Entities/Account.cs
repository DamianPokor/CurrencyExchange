using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.API.Data.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        
        public int UserId { get; set; }
        
        public string AccountNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        
        public string Currency { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nawigacje (opcjonalnie)
        public virtual User? User { get; set; }
        public virtual Currency? CurrencyNavigation { get; set; }
    }
}


/*public class Account
{
    public required int accountId{get, set}
        public required int userId{get, set}
    public required string accountNumber {get,set}
    public required int balance {get,set}
    public required string currency{get,set}
    public required string createdAt{get,se}
}*/

