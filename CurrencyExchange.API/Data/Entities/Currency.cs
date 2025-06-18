using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.API.Data.Entities
{
    public class Currency
    {
        public string Code { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;

        //Nawigacja
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
