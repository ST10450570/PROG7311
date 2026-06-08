namespace GLMS.Api.Services.Interfaces
{
    public interface IExchangeRateService
    {

      Task<decimal> GetUsdToZarRateAsync(); 

    }
}
