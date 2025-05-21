namespace CurrencyExchange.API.Data.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int FromAccID { get; set; }
    public int ToAccID { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public decimal? ExchangeRate { get; set; }
}