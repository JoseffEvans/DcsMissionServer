using Microsoft.EntityFrameworkCore;
using UnitManager.Models;

namespace UnitManager {
    public class UnitDbContext(DbContextOptions options) : DbContext(options) {
        public DbSet<Unit> Units { get; set; }
    }
}
