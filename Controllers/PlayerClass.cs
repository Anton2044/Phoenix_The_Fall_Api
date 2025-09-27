using System.ComponentModel.DataAnnotations;

namespace Phoenix_The_Fall_Web_application.Models
{
    public class PlayerClas
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string ClassName { get; set; }
        public string Description { get; set; }
    }
}
