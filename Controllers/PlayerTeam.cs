using System.ComponentModel.DataAnnotations;

namespace Phoenix_The_Fall_Web_application.Models
{
    public class PlayerTeam
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string TeamName { get; set; }
        public int TeamScore { get; set; }
    }
}
