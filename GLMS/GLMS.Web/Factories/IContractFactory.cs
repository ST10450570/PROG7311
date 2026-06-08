using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public interface IContractFactory
    {
        Contract CreateContract(int clientId, DateTime startDate, DateTime endDate);
    }
}
