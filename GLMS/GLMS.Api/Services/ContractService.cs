using GLMS.Api.DTOs.Contracts;
using GLMS.Api.Models;
using GLMS.Api.Observers;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Services.Interfaces;

namespace GLMS.Api.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _repository;
        private readonly ContractSubject _subject;

        public ContractService(IContractRepository repository, ContractSubject subject)
        {
            _repository = repository;
            _subject = subject;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync(string? status, DateTime? startDateFrom, DateTime? startDateTo)
        {
            ContractStatus? parsedStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ContractStatus>(status, true, out var result))
            {
                parsedStatus = result;
            }
            return await _repository.GetAllAsync(parsedStatus, startDateFrom, startDateTo);
        }

        public async Task<Contract?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Contract> CreateAsync(CreateContractDto dto)
        {
            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel
            };
            return await _repository.AddAsync(contract);
        }

        public async Task UpdateStatusAsync(int id, UpdateContractStatusDto dto)
        {
            var contract = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Contract not found.");
            contract.Status = dto.Status;
            await _repository.UpdateAsync(contract);
            _subject.NotifyAll(contract.Id, dto.Status.ToString());
        }

        public async Task UpdatePathAsync(int id, string path)
        {
            var contract = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Contract not found.");
            contract.SignedAgreementPath = path;
            await _repository.UpdateAsync(contract);
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}
