
using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User?> RegisterAsync(string email, string password, string firstName, string lastName);
    }
}


