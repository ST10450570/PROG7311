using GLMS.Api.Models;
using GLMS.Api.Repositories.Interfaces;
using GLMS.Api.Data;
using Microsoft.EntityFrameworkCore;


namespace GLMS.Api.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }
    }
}
