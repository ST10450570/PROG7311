using GLMS.Api.Models;

namespace GLMS.Api.Repositories.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<ServiceRequest> AddAsync(ServiceRequest request);
        Task DeleteAsync(int id);

    }
}
