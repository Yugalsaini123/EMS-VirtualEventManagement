// EMS.Services\Interfaces\ISpeakerService.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EMS.Services.Interfaces
{
    public interface ISpeakerService
    {
        Task<List<dynamic>> GetAllSpeakersAsync();
        Task<dynamic> GetSpeakerByIdAsync(Guid speakerId);
        Task<dynamic> CreateSpeakerAsync(string name, string email, string designation, 
            string organization, string bio, string phoneNumber, string linkedinUrl);
        Task<dynamic> UpdateSpeakerAsync(Guid speakerId, string name, string email, 
            string designation, string organization, string bio, string phoneNumber, string linkedinUrl);
        Task<bool> DeleteSpeakerAsync(Guid speakerId);
        Task<List<dynamic>> GetActiveSpeakersAsync();
        Task<List<dynamic>> SearchSpeakersByNameAsync(string name);
        Task<List<dynamic>> GetSpeakersWithSessionCountAsync();
    }
}