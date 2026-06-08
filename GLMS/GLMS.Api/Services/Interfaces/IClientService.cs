using GLMS.Api.DTOs.Clients;
using GLMS.Api.Models;

namespace GLMS.Api.Services.Interfaces
{
    public interface IClientService
    {

        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> CreateAsync(CreateClientDto dto);
        Task UpdateAsync(int id, CreateClientDto dto);
        Task DeleteAsync(int id);
    }
}
