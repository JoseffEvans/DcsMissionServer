﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitManager.Models {
    [Table("Unit")]
    public class Unit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitId { get; set; }
        public int CoalitionId { get; set; }
        public string? UnitName { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
    }
}
