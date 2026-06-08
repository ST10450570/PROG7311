using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public class StandardContractFactory : IContractFactory
    {
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate)
        {
            return new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ContractStatus.Draft,
                ServiceLevel = ServiceLevel.Standard
            };
        }
    }
}
