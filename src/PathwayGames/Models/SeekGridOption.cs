using SQLite;

namespace PathwayGames.Models
{
    [Table("SeekGridOptions")]
    public class SeekGridOption : ModelBase
    {
        public string Name { get; set; }

        public string TargetIdiom { get; set; }

        public int GridColumns { get; set; }

        public int GridRows { get; set; }

        public int ImmitationCount { get; set; }

        public int ContrastCount { get; set; }

        public bool IsDefault { get; set; }
    }
}
