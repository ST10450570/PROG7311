using GLMS.Web.Models;
using Xunit;

namespace GLMS.Tests
{
    public class WorkflowLogicTests
    {
        private static bool CanCreateServiceRequest(ContractStatus status)
            => status != ContractStatus.Expired && status != ContractStatus.OnHold;

        [Theory]
        [InlineData(ContractStatus.Expired, false)]
        [InlineData(ContractStatus.OnHold, false)]
        [InlineData(ContractStatus.Active, true)]
        [InlineData(ContractStatus.Draft, true)]
        public void ServiceRequest_CreationAllowed_BasedOnContractStatus(ContractStatus status, bool expected)
        {
            Assert.Equal(expected, CanCreateServiceRequest(status));
        }

        [Fact]
        public void Contract_DefaultStatus_IsDraft()
        {
            var contract = new Contract();
            Assert.Equal(ContractStatus.Draft, contract.Status);
        }

        [Fact]
        public void ServiceRequest_DefaultStatus_IsPending()
        {
            var sr = new ServiceRequest();
            Assert.Equal(ServiceRequestStatus.Pending, sr.Status);
        }
    }
}
