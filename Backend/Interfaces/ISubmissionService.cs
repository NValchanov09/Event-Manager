using Microsoft.AspNetCore.Identity.UI.Services;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;

namespace EventManagerBackend
{
    public interface ISubmissionService
    {
<<<<<<< Updated upstream:Backend/Interfaces/ISubmissionService.cs
        List<SubmissionSummaryDto> GetSubmissionsByEventId(int eventId);
        Submission? GetSubmissionByEventAndUser(int eventId, string userId);
        IResult Create(int eventId, string userId, CreateSubmissionDto dto);
        IResult UpdateSubmission(int eventId, string userId, UpdateSubmissionDto dto);
        Task<IResult> RemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
        Task<bool> AdminRemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
=======
        List<SubmitSummaryDto> GetSubmitsByEventId(int eventId);
        Submit? GetSubmitByEventAndUser(int eventId, string userId);
        IResult Create(int eventId, string userId, CreateSubmitDto dto);
        IResult UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto);
        Task<IResult> RemoveUserFromEvent(int eventId, string userId);
        Task<bool> AdminRemoveUserFromEvent(int eventId, string userId);
>>>>>>> Stashed changes:Backend/Interfaces/ISubmitService.cs
    }

}