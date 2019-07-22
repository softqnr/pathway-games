using SQLite;
namespace PathwayGames.Models
{
    public class ModelBase
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
    }
}
