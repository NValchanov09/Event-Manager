﻿using EventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventManagerBackend.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [Required]
        public DateTime? SignUpDeadline { get; set; }
        [Required]
        public string? Location { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "People limit must be a positive number.")]
        public int? PeopleLimit { get; set; }
        public IList<Field>? Fields { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        [Required]
        public DateTime? UpdatedAt { get; set; }
        [JsonIgnore]
        public ICollection<Submit>? Submissions { get; set; }

    }
}
