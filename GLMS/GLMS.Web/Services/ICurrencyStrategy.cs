namespace GLMS.Web.Services
{
    public interface ICurrencyStrategy
    {
        decimal Convert(decimal amount, decimal exchangeRate);
    }
}
