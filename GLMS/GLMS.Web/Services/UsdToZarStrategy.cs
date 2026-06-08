namespace GLMS.Web.Services
{
    public class UsdToZarStrategy : ICurrencyStrategy
    {
        public decimal Convert(decimal amount, decimal exchangeRate) => amount * exchangeRate;
    }
}
