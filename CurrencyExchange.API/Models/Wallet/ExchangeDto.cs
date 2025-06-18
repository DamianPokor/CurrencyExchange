namespace CurrencyExchange.API.Models.Wallet
{
    public class ExchangeDto
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
    }
}