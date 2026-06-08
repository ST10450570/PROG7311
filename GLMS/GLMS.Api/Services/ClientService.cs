using GLMS.Api.DTOs.Clients;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Services.Interfaces;

namespace GLMS.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Client>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Client?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Client> CreateAsync(CreateClientDto dto)
        {
            var client = new Client
            {
                Name = dto.Name,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                Region = dto.Region
            };
            return await _repository.AddAsync(client);
        }

        public async Task UpdateAsync(int id, CreateClientDto dto)
        {
            var client = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found.");
            client.Name = dto.Name;
            client.ContactEmail = dto.ContactEmail;
            client.ContactPhone = dto.ContactPhone;
            client.Region = dto.Region;
            await _repository.UpdateAsync(client);
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}
