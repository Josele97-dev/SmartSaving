using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> RegisterAsync(User user);

        Task<List<User>> GetAllUsersAsync();
    }
}
