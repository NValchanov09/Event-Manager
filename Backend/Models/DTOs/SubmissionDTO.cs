using EventManagerBackend.Models.JSON;

namespace EventManagerBackend.Models.DTOs
{
    public class SubmissionSummaryDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public bool? IsOnWaitingList { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IList<Answer>? Answers { get; set; }
    }

    public class CreateSubmissionDto
    {
        public IList<Answer>? Answers { get; set; }
    }

    public class UpdateSubmissionDto
    {
        public IList<Answer>? Answers { get; set; }
    }
}
