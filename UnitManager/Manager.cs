using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitManager.Models;

namespace UnitManager {
    public class Manager {

        readonly IServiceProvider _services;

        public Manager(IServiceProvider services) {
            _services = services;
            NewDb().Database.ExecuteSql($@"
                DROP TABLE Unit;
                CREATE TABLE Unit (
	                UnitId INTEGER PRIMARY KEY,
	                CoalitionId INTEGER NOT NULL,
	                UnitName TEXT NOT NULL,
                    PosX REAL,
                    PosY REAL
                );
            ");
        }

        public async Task<List<Unit>> GetAllUnits() =>
            await NewDb().Units.ToListAsync();

        public async Task<List<Unit>> GetUnits(int coalition) =>
            await NewDb().Units.Where(unit => unit.CoalitionId == coalition).ToListAsync();

        public async Task UpdateUnits(List<Unit> units) {
            var db = NewDb();
            await db.Units.UpsertRange(units).On(unit => unit.UnitId).RunAsync();

            var aliveIds = units.Select(unit => unit.UnitId);
            await db.Units
                .Where(unit => !aliveIds.Contains(unit.UnitId))
                .ExecuteDeleteAsync();
        }

        public async Task ClearDb() {
            await NewDb().Database.ExecuteSqlAsync($"DELETE FROM Unit; VACUUM;");
        }

        UnitDbContext NewDb() =>
            _services.CreateScope().ServiceProvider.GetService<UnitDbContext>()
                ?? throw new Exception("A db unit context service should have been added");
    }
}
