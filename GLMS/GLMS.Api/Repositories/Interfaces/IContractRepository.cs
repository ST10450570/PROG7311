using GLMS.Api.Models;

namespace GLMS.Api.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync(ContractStatus? status, DateTime? startDateFrom, DateTime? startDateTo);
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract> AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);

    }
}
