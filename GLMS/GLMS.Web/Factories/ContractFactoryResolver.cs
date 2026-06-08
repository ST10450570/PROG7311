using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public class ContractFactoryResolver
    {
        private readonly StandardContractFactory _standard = new();
        private readonly PremiumContractFactory _premium = new();

        public IContractFactory Resolve(ServiceLevel level) => level switch
        {
            ServiceLevel.Premium => _premium,
            _ => _standard
        };
    }
}
