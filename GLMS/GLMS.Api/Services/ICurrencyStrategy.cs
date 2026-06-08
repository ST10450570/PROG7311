namespace GLMS.Api.Services
{
    public interface ICurrencyStrategy
    {
        decimal Convert(decimal amount, decimal exchangeRate);
        decimal CalculateZarAmount(decimal usdAmount);
    }
}