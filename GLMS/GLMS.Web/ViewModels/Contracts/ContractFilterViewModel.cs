using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.Contracts
{
    public class ContractFilterViewModel
    {
        public ContractStatus? Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDateTo { get; set; }

        public List<ContractViewModel> Results { get; set; } = new();
    }
}