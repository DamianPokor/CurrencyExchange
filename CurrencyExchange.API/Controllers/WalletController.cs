using System.Security.Claims;
using CurrencyExchange.API.Data;
using CurrencyExchange.API.Data.Entities;
using CurrencyExchange.API.Models.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public WalletController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //metoda, aby pobrać ID zalogowanego usera
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        
        //WPŁATA
       
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Kwota musi być większa od zera.");

            var userId = GetCurrentUserId();

            // Sprawdź, czy waluta istnieje
            var currency = await _context.Currencies.FindAsync(dto.Currency);
            if (currency == null)
                return BadRequest("Ta waluta nie jest obsługiwana.");

            // Znajdź (lub utwórz) konto użytkownika w tej walucie
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Currency == dto.Currency);

            if (account == null)
            {
                // Wygeneruj unikalny numer konta (przykład: GUID)
                var newAccountNumber = GenerateAccountNumber();

                account = new Account
                {
                    UserId = userId,
                    AccountNumber = newAccountNumber,
                    Balance = 0m,
                    Currency = dto.Currency,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync(); // zapisz, aby mieć AccountId
            }

            // Dodaj kwotę do salda
            account.Balance += dto.Amount;
            _context.Accounts.Update(account);

            // Zapisz transakcję (wpłata to FromAccID == ToAccID)
            var tx = new Transaction
            {
                FromAccID = account.AccountId,
                ToAccID = account.AccountId,
                Amount = dto.Amount,
                ExchangeRate = null,
                TransactionDate = DateTime.UtcNow
            };
            _context.Transactions.Add(tx);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                account.AccountId,
                account.AccountNumber,
                account.Currency,
                account.Balance
            });
        }


        //WYMIANA WALUT (Exchange)

        [HttpPost("exchange")]
        public async Task<IActionResult> Exchange([FromBody] ExchangeDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Kwota musi być większa od zera.");

            if (dto.FromCurrency == dto.ToCurrency)
                return BadRequest("Waluty muszą być różne.");

            var userId = GetCurrentUserId();

            // 1) Sprawdź, czy obie waluty są w tabeli Currency
            var fromCurrencyEntity = await _context.Currencies.FindAsync(dto.FromCurrency);
            var toCurrencyEntity = await _context.Currencies.FindAsync(dto.ToCurrency);
            if (fromCurrencyEntity == null || toCurrencyEntity == null)
                return BadRequest("Przynajmniej jedna z walut nie jest obsługiwana.");

            // 2) Pobierz konto „z” i konto „do”
            var fromAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Currency == dto.FromCurrency);
            if (fromAccount == null || fromAccount.Balance < dto.Amount)
                return BadRequest("Brak wystarczających środków na koncie „z”.");

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Currency == dto.ToCurrency);

            // 3) Pobierz kurs wymiany (tu przykład na sztywno lub zewnętrzne API)
            decimal rate = await GetExchangeRateAsync(dto.FromCurrency, dto.ToCurrency);
            decimal receivedAmount = Math.Round(dto.Amount * rate, 2); // zaokrąglamy do 2 miejsc po przecinku

            // 4) Odejmij z konta „z”
            fromAccount.Balance -= dto.Amount;
            _context.Accounts.Update(fromAccount);

            // 5) Jeżeli konto „do” nie istnieje, utwórz je
            if (toAccount == null)
            {
                var newAccountNumber = GenerateAccountNumber();
                toAccount = new Account
                {
                    UserId = userId,
                    AccountNumber = newAccountNumber,
                    Balance = 0m,
                    Currency = dto.ToCurrency,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Accounts.Add(toAccount);
                await _context.SaveChangesAsync(); // zapisujemy, aby pozyskać ToAccID
            }

            // 6) Dodaj środki do konta „do”
            toAccount.Balance += receivedAmount;
            _context.Accounts.Update(toAccount);

            // 7) Zapisz transakcję wymiany
            var tx = new Transaction
            {
                FromAccID = fromAccount.AccountId,
                ToAccID = toAccount.AccountId,
                Amount = dto.Amount,        // kwota w walucie FromCurrency
                ExchangeRate = rate,
                TransactionDate = DateTime.UtcNow
            };
            _context.Transactions.Add(tx);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                From = new
                {
                    fromAccount.AccountId,
                    fromAccount.AccountNumber,
                    fromAccount.Currency,
                    NewBalance = fromAccount.Balance
                },
                To = new
                {
                    toAccount.AccountId,
                    toAccount.AccountNumber,
                    toAccount.Currency,
                    NewBalance = toAccount.Balance
                },
                Exchanged = dto.Amount,
                Rate = rate,
                Received = receivedAmount
            });
        }


        //Pobranie sald i historii transakcji

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalances()
        {
            var userId = GetCurrentUserId();

            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.AccountId,
                    a.AccountNumber,
                    a.Currency,
                    a.Balance
                })
                .ToListAsync();

            return Ok(accounts);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = GetCurrentUserId();

            // Pobieramy wszystkie transakcje, dla których FromAccID lub ToAccID należy do usera
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.AccountId)
                .ToListAsync();

            var txs = await _context.Transactions
                .Where(t => userAccountIds.Contains(t.FromAccID) 
                         || userAccountIds.Contains(t.ToAccID))
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new
                {
                    t.TransactionId,
                    t.FromAccID,
                    t.ToAccID,
                    t.Amount,
                    t.ExchangeRate,
                    t.TransactionDate
                })
                .ToListAsync();

            return Ok(txs);
        }

   
        // Metoda pomocnicza: kurs wymiany
        private async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            // W przykładzie używam prostych, sztywnych wartości.
            // W produkcji możesz:
            // - Pobierać kursy z zewnętrznego serwisu (np. fixer.io, exchangerate-api itp.)
            // - Trzymać tabelę kursów w bazie danych i odświeżać je co X minut
            await Task.Yield();

            if (fromCurrency == "USD" && toCurrency == "EUR") return 0.90m;
            if (fromCurrency == "EUR" && toCurrency == "USD") return 1.10m;
            if (fromCurrency == "USD" && toCurrency == "PLN") return 4.00m;
            if (fromCurrency == "PLN" && toCurrency == "USD") return 0.25m;
            if (fromCurrency == "EUR" && toCurrency == "PLN") return 4.20m;
            if (fromCurrency == "PLN" && toCurrency == "EUR") return 0.24m;

            throw new InvalidOperationException($"Brak zdefiniowanego kursu z {fromCurrency} na {toCurrency}.");
        }
        
        // Metoda pomocnicza: generowanie numeru konta (przykład)
        private string GenerateAccountNumber()
        {
            // Możesz tu wstawić dowolny generator numeru rachunku,
            // np. GUID, albo format IBAN, albo własne ID z prefiksem.
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
