using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSaving.Data;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            using (var context = new AppDbContext())
            {
                return await context.Users
                    .Include(u => u.Accounts)
                    .ThenInclude(a => a.Categories)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using (var context = new AppDbContext())
            {
                return await context.Users
                    .Include(u => u.Accounts)
                    .ThenInclude(a => a.Categories)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Add(user);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using (var context = new AppDbContext())
            {
                return await context.Users
                    .Include(u => u.Accounts)
                    .ToListAsync();
            }
        }
    }
}
