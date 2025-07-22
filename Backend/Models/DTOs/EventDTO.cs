using EventManagerBackend.Models.JSON;
using Microsoft.AspNetCore.Mvc;

namespace EventManagerBackend.Models.DTOs
{
    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public int? SpotsLeft { get; set; }
        public bool UserSignedUp { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class EventDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public int? SpotsLeft { get; set; }
        public bool UserSignedUp { get; set; }
        public string? ImageUrl { get; set; }
        public IList<Field>? Fields { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateEventDto
    {
        [FromForm] public string? Name { get; set; }
        [FromForm] public string? Description { get; set; }
        [FromForm] public DateTime? Date { get; set; }
        [FromForm] public DateTime? SignUpDeadline { get; set; }
        [FromForm] public string? Location { get; set; }
        [FromForm] public int? PeopleLimit { get; set; }
        [FromForm] public IFormFile? Image { get; set; }
        [FromForm] public IList<Field>? Fields { get; set; }
    }

    public class UpdateEventDto
    {
        [FromForm] public string? Name { get; set; }
        [FromForm] public string? Description { get; set; }
        [FromForm] public DateTime? Date { get; set; }
        [FromForm] public DateTime? SignUpDeadline { get; set; }
        [FromForm] public string? Location { get; set; }
        [FromForm] public int? PeopleLimit { get; set; }
        [FromForm] public IFormFile? Image { get; set; }
        [FromForm] public IList<Field>? Fields { get; set; }
    }

}
