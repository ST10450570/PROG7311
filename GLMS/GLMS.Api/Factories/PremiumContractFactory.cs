using GLMS.Api.Models;

namespace GLMS.Api.Factories
{
    public class PremiumContractFactory : IContractFactory
    {
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate)
        {
            return new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ContractStatus.Draft,
                ServiceLevel = ServiceLevel.Premium
            };
        }
    }
}
