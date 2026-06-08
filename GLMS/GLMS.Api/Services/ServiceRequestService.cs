using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Services.Interfaces;

namespace GLMS.Api.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;
        private readonly IContractRepository _contractRepository;
        private readonly IExchangeRateService _exchangeRateService;

        public ServiceRequestService(IServiceRequestRepository repository, IContractRepository contractRepository, IExchangeRateService exchangeRateService)
        {
            _repository = repository;
            _contractRepository = contractRepository;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<ServiceRequest> CreateAsync(CreateServiceRequestDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(dto.ContractId)
                ?? throw new KeyNotFoundException("Contract not found.");

            if (contract.Status != ContractStatus.Active)
            {
                throw new InvalidOperationException("Service requests can only be created against Active contracts.");
            }

            decimal currentRate = await _exchangeRateService.GetUsdToZarRateAsync();

            var request = new ServiceRequest
            {
                ContractId = dto.ContractId,
                Description = dto.Description,
                CostUsd = dto.CostUsd,
                CostZar = dto.CostUsd * currentRate,
                Status = ServiceRequestStatus.Pending,
                CreatedOn = DateTime.UtcNow
            };

            return await _repository.AddAsync(request);
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}
