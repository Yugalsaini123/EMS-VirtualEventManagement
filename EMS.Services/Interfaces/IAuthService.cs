//EMS.Services/Interfaces/IAuthService.cs
using System.Threading.Tasks;
 
namespace EMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string userName, string password);
        Task<string> RefreshTokenAsync(string oldToken);
        Task<bool> LogoutAsync(string email);
        Task<bool> ValidateCredentialsAsync(string email, string password);
    }
}