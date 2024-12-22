using UnitManager.Models;

namespace DcsMissionServer.Models {
    public class UnitsViewModel {
        public required IEnumerable<Unit> Units { get; set; }
        public IEnumerable<Unit> BlueUnits { get => Units.Where(unit => unit.CoalitionId == 1); }
        public IEnumerable<Unit> RedUnits { get => Units.Where(unit => unit.CoalitionId == 2); }
    }
}
