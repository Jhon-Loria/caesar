using Microsoft.EntityFrameworkCore;

namespace caesar.data
{
    public class CaesarDbContext : DbContext
    {
        public CaesarDbContext(DbContextOptions<CaesarDbContext> options) : base(options) { }

        public DbSet<CaesarMessage> CaesarMessages { get; set; }
    }
}
