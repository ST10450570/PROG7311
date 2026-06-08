using GLMS.Api.Models;

namespace GLMS.Api.Factories
{
    public interface IContractFactory
    {
        Contract CreateContract(int clientId, DateTime startDate, DateTime endDate);
    }
}
