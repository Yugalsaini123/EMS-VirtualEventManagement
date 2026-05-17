// EMS.Services\Implementations\SpeakerService.cs
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;
 
namespace EMS.Services.Implementations
{
    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _speakerRepository;
 
        public SpeakerService(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }
 
        public async Task<List<dynamic>> GetAllSpeakersAsync()
        {
            var speakers = await _speakerRepository.GetAllAsync();
 
            return speakers
            .Where(s => s.IsActive)
            .Select(s => (dynamic)new
            {
                s.SpeakerId,
                s.SpeakerName,
                s.Email,
                s.Designation,
                s.Organization,
                s.IsActive
            }).ToList();
        }
 
        public async Task<dynamic> GetSpeakerByIdAsync(Guid speakerId)
        {
            var speaker = await _speakerRepository.GetByIdAsync(speakerId);
 
            if (speaker == null)
                throw new KeyNotFoundException("Speaker not found");
 
            return new
            {
                speaker.SpeakerId,
                speaker.SpeakerName,
                speaker.Email,
                speaker.Designation,
                speaker.Organization,
                speaker.Bio,
                speaker.PhoneNumber,
                speaker.LinkedInUrl,
                speaker.IsActive
            };
        }
 
        public async Task<dynamic> CreateSpeakerAsync(string name, string email, string designation,
            string organization, string bio, string phoneNumber, string linkedinUrl)
        {
            var newSpeaker = new SpeakersDetails
            {
                SpeakerId = Guid.NewGuid(),
                SpeakerName = name,
                Email = email,
                Designation = designation,
                Organization = organization,
                Bio = bio,
                PhoneNumber = phoneNumber,
                LinkedInUrl = linkedinUrl,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
 
            await _speakerRepository.AddAsync(newSpeaker);
 
            return new
            {
                newSpeaker.SpeakerId,
                newSpeaker.SpeakerName,
                newSpeaker.Email,
                newSpeaker.IsActive
            };
        }
 
        public async Task<dynamic> UpdateSpeakerAsync(Guid speakerId, string name, string email,
            string designation, string organization, string bio, string phoneNumber, string linkedinUrl)
        {
            var speaker = await _speakerRepository.GetByIdAsync(speakerId);
 
            if (speaker == null)
                throw new KeyNotFoundException("Speaker not found");
 
            speaker.SpeakerName = name;
            speaker.Email = email;
            speaker.Designation = designation;
            speaker.Organization = organization;
            speaker.Bio = bio;
            speaker.PhoneNumber = phoneNumber;
            speaker.LinkedInUrl = linkedinUrl;
 
            await _speakerRepository.UpdateAsync(speaker);
 
            return new
            {
                speaker.SpeakerId,
                speaker.SpeakerName,
                speaker.Email
            };
        }
 
        public async Task<bool> DeleteSpeakerAsync(Guid speakerId)
        {
            var speaker = await _speakerRepository.GetByIdAsync(speakerId);
 
            if (speaker == null)
                throw new KeyNotFoundException("Speaker not found");
 
            speaker.IsActive = false;
            await _speakerRepository.UpdateAsync(speaker);
            return true;
        }
 
        public async Task<List<dynamic>> GetActiveSpeakersAsync()
        {
            var speakers = await _speakerRepository.GetActiveSpeakersAsync();
 
            return speakers.Select(s => (dynamic)new
            {
                s.SpeakerId,
                s.SpeakerName,
                s.Email,
                s.Designation,
                s.Organization
            }).ToList();
        }
 
        public async Task<List<dynamic>> SearchSpeakersByNameAsync(string name)
        {
            var speakers = await _speakerRepository.SearchByNameAsync(name);
 
            return speakers.Select(s => (dynamic)new
            {
                s.SpeakerId,
                s.SpeakerName,
                s.Email,
                s.Designation
            }).ToList();
        }

        public async Task<List<dynamic>> GetSpeakersWithSessionCountAsync()
        {
            var speakers = await _speakerRepository.GetSpeakersWithSessionCountAsync();

            return speakers.Select(s => (dynamic)new
            {
                s.SpeakerId,
                s.SpeakerName,
                s.Email,
                s.Designation,
                s.Organization,
                s.SessionCount
            }).ToList();
        }
    }
}