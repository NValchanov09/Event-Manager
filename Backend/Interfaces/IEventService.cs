using Microsoft.AspNetCore.Identity.UI.Services;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;

namespace EventManagerBackend
{
    public interface IEventService
    {
            bool Create(Event newEvent);
            Event? GetEventById(int eventId);
            EventDetailsDto? GetEventById(int eventId, string userId);
            List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, string userId, bool alphabetical = false, bool sortDescending = false);
            List<EventSummaryDto> GetJoinedEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, string userId, bool alphabetical = false, bool sortDescending = false);
            Task<bool> RemoveById(int eventId);
            Task<bool> Update(int eventId, UpdateEventDto dto);
            bool Exists(int eventId);
    }
}