//EMS.Services\Interfaces\ISessionService.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EMS.Services.Interfaces
{
    public interface ISessionService
    {
        Task<List<dynamic>> GetSessionsByEventAsync(Guid eventId);
        Task<List<dynamic>> SearchSessionsByEventAsync(Guid eventId, string searchTerm);
        Task<dynamic> GetSessionByIdAsync(Guid sessionId);
        Task<dynamic> CreateSessionAsync(Guid eventId, string title, DateTime startTime, 
            DateTime endTime, string description, string location, Guid? speakerId, string sessionUrl);
        Task<dynamic> UpdateSessionAsync(Guid sessionId, string title, DateTime startTime, 
            DateTime endTime, string description, string location, Guid? speakerId, string sessionUrl);
        Task<bool> DeleteSessionAsync(Guid sessionId);
        Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId);
        Task<bool> RemoveSpeakerAsync(Guid sessionId);
    }
}