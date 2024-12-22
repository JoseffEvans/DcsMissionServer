using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitManager.Models {
    [Table("Unit")]
    public class Unit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitId { get; set; }
        public int CoalitionId { get; set; }
        public required string UnitName { get; set; }
    }
}
