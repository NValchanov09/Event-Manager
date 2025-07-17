using EventManagerBackend.Models.JSON;

namespace EventManagerBackend.Models.DTOs
{
    public static class SubmissionMapper
    {
        public static Submission ToEntity(int eventId, string userId, bool isOnWaitingList, CreateSubmissionDto dto)
        {
            return new Submission
            {
                EventId = eventId,
                UserId = userId,
                Answers = dto.Answers?.Select(a => new Answer
                {
                    Id = a.Id,
                    Name = a.Name,
                    Options = a.Options
                }).ToList(),
                IsOnWaitingList = isOnWaitingList,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(Submission submission, UpdateSubmissionDto dto)
        {
            if (dto.Answers != null)
            {
                submission.Answers = dto.Answers.Select(a => new Answer
                {
                    Id = a.Id,
                    Name = a.Name,
                    Options = a.Options
                }).ToList();
                submission.UpdatedAt = DateTime.UtcNow;
            }
        }

        public static SubmissionSummaryDto ToSummaryDto(this Submission submission)
        {
            return new SubmissionSummaryDto
            {
                UserId = submission.UserId,
                Email = submission.User?.Email,
                CreatedAt = submission.CreatedAt,
                Answers = submission.Answers
            };
        }
    }
}
