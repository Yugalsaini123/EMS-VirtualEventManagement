//EMS.Services/Interfaces/IEventService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Services.Helpers;
using EMS.DAL.Models;

namespace EMS.Services.Interfaces
{
    public interface IEventService
    {
        Task<PaginatedResponse<EventListDto>> GetAllEventsAsync(int pageNumber, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null);
        Task<dynamic> GetEventByIdAsync(Guid eventId);
        Task<dynamic> CreateEventAsync(string eventName, string category, DateTime eventDate, 
            string description, string location, int? maxParticipants);
        Task<dynamic> UpdateEventAsync(Guid eventId, string eventName, string category, 
            DateTime eventDate, string description, string location, int? maxParticipants);
        Task<bool> DeleteEventAsync(Guid eventId);
        Task<List<string>> GetCategoriesAsync();
        Task<List<dynamic>> GetEventsByCategoryAsync(string category);
        Task<List<dynamic>> GetUpcomingEventsAsync();
        Task<List<dynamic>> GetAllEventsAdminAsync();
        Task<bool> ToggleEventStatusAsync(Guid eventId);
    }
}