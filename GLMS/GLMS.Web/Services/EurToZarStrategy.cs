namespace GLMS.Web.Services
{
    public class EurToZarStrategy : ICurrencyStrategy
    {
        public decimal Convert(decimal amount, decimal exchangeRate) => amount * exchangeRate;
    }
}
