#pragma warning disable IDE1006 // Naming Styles

namespace DcsModels {
    public class GroundGroup {
        public class Route {
            public class Point {

            }

            public IEnumerable<Point> points { get; set; } = [];
        }

        public class GroundUnit {
            public class Skills {
                public const string High = "High";
            }

            public required int unitId { get; set; }
            public required string name { get; set; }
            public required string type { get; set; }

            public required double x { get; set; }
            public required double y { get; set; }
            public int heading { get; set; } = 0;

            public bool playerCanDrive { get; set; }

            public string skill { get; set; } = Skills.High;
        }

        public required int groupId { get; set; }
        public required string name { get; set; }

        public required double x { get; set; }
        public required double y { get; set; }

        public bool visible { get; set; } = true;
        public bool taskSelected { get; set; }
        public bool uncontrollable { get; set; }
        public bool hidden { get; set; } = false;

        public Route route { get; set; } = new();
        public required IEnumerable<GroundUnit> units { get; set; }

        public int start_time { get; set; } = 0;
        public string task { get; set; } = "Ground Nothing";
    }
}
