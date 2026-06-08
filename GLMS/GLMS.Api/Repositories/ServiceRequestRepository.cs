using GLMS.Api.Data;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<ServiceRequest> AddAsync(ServiceRequest request)
        {
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request != null)
            {
                _context.ServiceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
