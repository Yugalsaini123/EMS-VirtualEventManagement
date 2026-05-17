// EMS.Services\Interfaces\IUserService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<dynamic>> GetAllUsersAsync();
        Task<dynamic> GetUserByEmailAsync(string email);
        Task<dynamic> UpdateUserAsync(string email, string userName, string password);
        Task<bool> DeleteUserAsync(string email);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<List<dynamic>> GetUsersByRoleAsync(string role);
        Task<List<dynamic>> GetAllParticipantsAsync();
    }
}