namespace GLMS.Api.Services
{
    public class CurrencyContext
    {
        private ICurrencyStrategy _strategy;

        public CurrencyContext(ICurrencyStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(ICurrencyStrategy strategy) => _strategy = strategy;

        public decimal Convert(decimal amount, decimal exchangeRate) => _strategy.Convert(amount, exchangeRate);
    }
}
