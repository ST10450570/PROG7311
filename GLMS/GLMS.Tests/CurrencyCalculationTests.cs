using GLMS.Web.Services;
using Xunit;

namespace GLMS.Tests
{
    public class CurrencyCalculationTests
    {
        private readonly CurrencyContext _context;

        public CurrencyCalculationTests()
        {
            _context = new CurrencyContext(new UsdToZarStrategy());
        }

        [Fact]
        public void Convert_PositiveAmount_ReturnsCorrectZarValue()
        {
            var result = _context.Convert(100m, 18.50m);
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void Convert_ZeroAmount_ReturnsZero()
        {
            var result = _context.Convert(0m, 18.50m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public void Convert_FractionalAmount_ReturnsCorrectValue()
        {
            var result = _context.Convert(1.50m, 18.00m);
            Assert.Equal(27.00m, result);
        }

        [Fact]
        public void Convert_ZeroRate_ReturnsZero()
        {
            var result = _context.Convert(500m, 0m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public void Convert_LargeAmount_ReturnsCorrectValue()
        {
            var result = _context.Convert(50000m, 18.50m);
            Assert.Equal(925000.00m, result);
        }

        [Fact]
        public void SetStrategy_SwitchToEur_ConvertesWithNewStrategy()
        {
            _context.SetStrategy(new EurToZarStrategy());
            var result = _context.Convert(100m, 20.00m);
            Assert.Equal(2000.00m, result);
        }
    }
}
