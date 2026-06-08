namespace GLMS.Api.Services
{
    public class PremiumCurrencyStrategy : ICurrencyStrategy
    {
        private const decimal Rate = 19.00m * 1.05m;

        public decimal Convert(decimal amount, decimal exchangeRate) => amount * exchangeRate;

        public decimal CalculateZarAmount(decimal usdAmount) => Math.Round(usdAmount * Rate, 2);
    }
}