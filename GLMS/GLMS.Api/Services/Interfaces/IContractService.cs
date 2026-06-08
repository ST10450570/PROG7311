using GLMS.Api.DTOs.Contracts;
using GLMS.Api.Models;


namespace GLMS.Api.Services.Interfaces
{
    public interface IContractService
    {

        Task<IEnumerable<Contract>> GetAllAsync(string? status, DateTime? startDateFrom, DateTime? startDateTo);
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract> CreateAsync(CreateContractDto dto);
        Task UpdateStatusAsync(int id, UpdateContractStatusDto dto);
        Task UpdatePathAsync(int id, string path);
        Task DeleteAsync(int id);
    }
}
