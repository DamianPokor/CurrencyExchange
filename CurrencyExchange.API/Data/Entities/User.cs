using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CurrencyExchange.API.Data.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nawigacja do kont
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}