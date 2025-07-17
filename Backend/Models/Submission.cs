using EventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagerBackend.Models
{
    public class Submission
    {
        [Key]
        public int EventId { get; set; }
        [Key]
        public string? UserId { get; set; }
        public IList<Answer>? Answers { get; set; }
        [Required]
        public bool? IsOnWaitingList { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        [Required]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("EventId")]
        [JsonIgnore]
        public Event? Event { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }
    }
}
