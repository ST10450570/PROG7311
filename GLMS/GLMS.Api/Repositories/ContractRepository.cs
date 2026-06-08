using GLMS.Api.Data;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace GLMS.Api.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly AppDbContext _context;

        public ContractRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync(ContractStatus? status, DateTime? startDateFrom, DateTime? startDateTo)
        {
            var query = _context.Contracts.AsQueryable();

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (startDateFrom.HasValue)
                query = query.Where(c => c.StartDate >= startDateFrom.Value);

            if (startDateTo.HasValue)
                query = query.Where(c => c.StartDate <= startDateTo.Value);

            return await query.ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract> AddAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }
    }
}
