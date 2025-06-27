using caesar.data;
using Microsoft.EntityFrameworkCore;

namespace caesar.data
{
    public class CaesarMessageRepository
    {
        private readonly CaesarDbContext _context;
        public CaesarMessageRepository(CaesarDbContext context)
        {
            _context = context;
        }

        public async Task<List<CaesarMessage>> GetAllAsync() => await _context.CaesarMessages.ToListAsync();
        public async Task<CaesarMessage?> GetByIdAsync(int id) => await _context.CaesarMessages.FindAsync(id);
        public async Task AddAsync(CaesarMessage message)
        {
            _context.CaesarMessages.Add(message);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(CaesarMessage message)
        {
            _context.CaesarMessages.Update(message);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var message = await _context.CaesarMessages.FindAsync(id);
            if (message != null)
            {
                _context.CaesarMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
