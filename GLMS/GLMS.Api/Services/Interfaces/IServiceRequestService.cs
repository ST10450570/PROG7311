using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Models;

namespace GLMS.Api.Services.Interfaces
{
    public interface IServiceRequestService
    {

        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<ServiceRequest> CreateAsync(CreateServiceRequestDto dto);
        Task DeleteAsync(int id);
    }
}
