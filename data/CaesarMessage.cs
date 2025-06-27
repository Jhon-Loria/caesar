using System.ComponentModel.DataAnnotations;

namespace caesar.data
{
    public class CaesarMessage
    {
        [Key]
        public int Id { get; set; }
        public string? OriginalMessage { get; set; }
        public string? EncryptedMessage { get; set; }
        public int Shift { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
