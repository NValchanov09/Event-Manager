using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using EventManagerBackend;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;

public class SubmissionService : ISubmissionService
{
    private readonly ApplicationDbContext _context;

    public SubmissionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<SubmissionSummaryDto> GetSubmissionsByEventId(int eventId)
    {
        return _context.Submissions
            .Include(s => s.User)
            .Where(s => s.EventId == eventId)
            .Select(s => SubmissionMapper.ToSummaryDto(s))
            .ToList();
    }

    public Submission? GetSubmissionByEventAndUser(int eventId, string userId)
    {
        return _context.Submissions
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);
    }

    public IResult Create(int eventId, string userId, CreateSubmissionDto dto)
    {
        if (!_context.Events.Any(e => e.Id == eventId))
            return Results.BadRequest(new { error = "Събитието не е намерено!" });

        if (_context.Submissions.Any(s => s.EventId == eventId && s.UserId == userId))
            return Results.Conflict(new { error = "Потребителят е вече записан за това събитие!" });

        if (_context.Events.Find(eventId).SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за записване е изтекъл!"});

        bool isOnWaitingList = _context.Events.Find(eventId).PeopleLimit <= _context.Submissions
            .Count(s => s.EventId == eventId);

        var entity = SubmissionMapper.ToEntity(eventId, userId, isOnWaitingList, dto);
        _context.Submissions.Add(entity);
        return _context.SaveChanges() > 0 ? Results.Created($"/submissions/{eventId}", dto) : Results.InternalServerError("Failed to save submission");
    }

    public IResult UpdateSubmission(int eventId, string userId, UpdateSubmissionDto dto)
    {
        var submission = _context.Submissions
            .Include(s => s.Event)
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);

        if (submission == null)
            return Results.BadRequest(new { error = "Не е намерена заявка"});

        if (submission.Event.SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за записване е изтекъл!" });

        SubmissionMapper.UpdateEntity(submission, dto);
        submission.UpdatedAt = DateTime.UtcNow;

        _context.Submissions.Update(submission);
        return _context.SaveChanges() > 0 ? Results.Created($"/submissions/{submission.EventId}", new { submission }) : Results.InternalServerError(new { error = "Oбновяването на заявката мина неуспешно." });
    }


    // Admin removes user from an Event
    public async Task<bool> AdminRemoveUserFromEvent(int eventId, string userId)
    {
        // Fetch submission with related user and event data
        var submission = await _context.Submissions
            .Include(s => s.User)
            .Include(s => s.Event)
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);

        if (submission?.User == null || submission.Event == null)
            return false;

        // Remove the submission
        _context.Submissions.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        return success;
    }
    //User removes himself from the event
    public async Task<IResult> RemoveUserFromEvent(int eventId, string userId)
    {
        // Fetch submission with related user and event data
        var submission = await _context.Submissions
            .Include(s => s.User)
            .Include(s => s.Event)
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);

        if (submission == null)
            return Results.BadRequest();

        if (submission?.User == null || submission.Event == null)
            return Results.InternalServerError();

        if (_context.Events.Find(eventId).SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за отписване е изтекъл!" });

        // Remove the submission
        _context.Submissions.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        return success ? Results.Ok() : Results.InternalServerError();
    }
}

