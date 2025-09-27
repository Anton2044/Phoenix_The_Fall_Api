using System.ComponentModel.DataAnnotations;

namespace Phoenix_The_Fall_Web_application.Models
{
    public class MapData
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string MapName { get; set; }
        public string Description { get; set; }
        public float MapSize { get; set; }
        public int SpawnPointsCount { get; set; }
    }
}
