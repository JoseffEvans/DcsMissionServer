using UnitManager.Models;

namespace CommandHandling {
    public class UpdateUnitsRequest {
        public required List<Unit> Units { get; set; }
    }
}
