namespace GLMS.Api.Services
{
    public class CurrencyContext
    {
        private ICurrencyStrategy? _strategy;

        public CurrencyContext() { }

        public CurrencyContext(ICurrencyStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(ICurrencyStrategy strategy) => _strategy = strategy;

        public decimal Convert(decimal amount, decimal exchangeRate)
        {
            if (_strategy == null) throw new InvalidOperationException("No currency strategy set.");
            return _strategy.Convert(amount, exchangeRate);
        }

        public decimal CalculateCostZar(decimal usdAmount)
        {
            if (_strategy == null) throw new InvalidOperationException("No currency strategy set.");
            if (usdAmount < 0) throw new ArgumentException("Amount cannot be negative.");
            return _strategy.CalculateZarAmount(usdAmount);
        }
    }
}