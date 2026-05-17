// EMS.Services\Interfaces\IParticipantService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EMS.Services.Interfaces
{
    public interface IParticipantService
    {
        Task<bool> RegisterForEventAsync(string email, Guid eventId);
        Task<List<dynamic>> GetRegisteredEventsAsync(string email);
        Task<bool> UnregisterFromEventAsync(string email, Guid eventId);
        Task<bool> MarkAttendanceAsync(Guid registrationId);
        Task<dynamic> GetRegistrationByIdAsync(Guid registrationId);
        Task<bool> SubmitFeedbackAsync(Guid registrationId, int rating, string feedback);
        Task<List<dynamic>> GetEventParticipantsAsync(Guid eventId);
        Task<bool> IsRegisteredAsync(string email, Guid eventId);
        Task<dynamic> GetAttendanceStatsAsync(Guid eventId);
    }
}