using Microsoft.AspNetCore.Identity.UI.Services;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;

namespace EventManagerBackend
{
    public interface ISubmissionService
    {
        List<SubmissionSummaryDto> GetSubmissionsByEventId(int eventId);
        Submission? GetSubmissionByEventAndUser(int eventId, string userId);
        IResult Create(int eventId, string userId, CreateSubmissionDto dto);
        IResult UpdateSubmission(int eventId, string userId, UpdateSubmissionDto dto);
        Task<IResult> RemoveUserFromEvent(int eventId, string userId);
        Task<bool> AdminRemoveUserFromEvent(int eventId, string userId);
    }

}