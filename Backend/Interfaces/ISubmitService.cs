using Microsoft.AspNetCore.Identity.UI.Services;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;

namespace EventManagerBackend
{
    public interface ISubmitService
    {
        List<SubmitSummaryDto> GetSubmitsByEventId(int eventId);
        Submit? GetSubmitByEventAndUser(int eventId, string userId);
        IResult Create(int eventId, string userId, CreateSubmitDto dto);
        IResult UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto);
        Task<IResult> RemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
        Task<bool> AdminRemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
    }

}