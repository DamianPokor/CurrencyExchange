namespace CurrencyExchange.API.Models.Wallet
{
    public class DepositDto
    {
        public string Currency { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
    }
}