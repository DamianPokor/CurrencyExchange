using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.API.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }//do wywalenia na poczet dziedziczonego id z identity
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string eMail { get; set;  } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   

        // Nawigacja
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
