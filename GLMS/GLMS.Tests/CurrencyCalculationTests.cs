using GLMS.Api.Services;
using System;
using Xunit;

namespace GLMS.Tests;

public class CurrencyCalculationTests
{
    [Theory]
    [InlineData(100, 1900)]    // Test with whole number
    [InlineData(50.50, 959.50)] // Test with decimal
    [InlineData(0, 0)]          // Test with zero
    [InlineData(1000, 19000)]   // Test with larger number
    [InlineData(1.99, 37.81)]   // Test with small decimal
    public void UsdToZar_StandardServiceLevel_CalculatesCorrectly(decimal usdAmount, decimal expectedZar)
    {
        // Arrange
        var currencyContext = new CurrencyContext();
        currencyContext.SetStrategy(new StandardCurrencyStrategy());

        // Act
        var result = currencyContext.CalculateCostZar(usdAmount);

        // Assert
        Assert.Equal(expectedZar, result);
    }

    [Theory]
    [InlineData(100, 1950)]    // Test with whole number - premium has 5% uplift on 19 rate = 19.95
    [InlineData(50.50, 984.98)] // Test with decimal (rounded to 2 decimal places)
    [InlineData(0, 0)]          // Test with zero
    [InlineData(1000, 19500)]   // Test with larger number
    [InlineData(1.99, 38.81)]  // Test with small decimal
    public void UsdToZar_PremiumServiceLevel_CalculatesCorrectly(decimal usdAmount, decimal expectedZar)
    {
        // Arrange
        var currencyContext = new CurrencyContext();
        currencyContext.SetStrategy(new PremiumCurrencyStrategy());

        // Act
        var result = currencyContext.CalculateCostZar(usdAmount);

        // Assert
        Assert.Equal(expectedZar, result);
    }

    [Fact]
    public void UsdToZar_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var currencyContext = new CurrencyContext();
        currencyContext.SetStrategy(new StandardCurrencyStrategy());
        decimal negativeAmount = -100;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => currencyContext.CalculateCostZar(negativeAmount));
    }

    [Fact]
    public void CurrencyContext_NoStrategySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var currencyContext = new CurrencyContext();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => currencyContext.CalculateCostZar(100));
    }

    [Theory]
    [InlineData(100, 19.00)]   // Standard rate check
    [InlineData(500, 19.00)]   // Consistency check
    public void StandardStrategy_AppliesCorrectExchangeRate(decimal usdAmount, decimal expectedRate)
    {
        // Arrange
        var strategy = new StandardCurrencyStrategy();

        // Act
        var result = strategy.CalculateZarAmount(usdAmount);
        var effectiveRate = result / usdAmount;

        // Assert
        Assert.Equal(expectedRate, effectiveRate);
    }

    [Theory]
    [InlineData(100, 19.95)]   // Premium rate check (19 * 1.05)
    [InlineData(500, 19.95)]   // Consistency check
    public void PremiumStrategy_AppliesCorrectExchangeRate(decimal usdAmount, decimal expectedRate)
    {
        // Arrange
        var strategy = new PremiumCurrencyStrategy();

        // Act
        var result = strategy.CalculateZarAmount(usdAmount);
        var effectiveRate = result / usdAmount;

        // Assert
        Assert.Equal(expectedRate, effectiveRate);
    }
}