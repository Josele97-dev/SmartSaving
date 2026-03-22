namespace SmartSaving.Repositories
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
    }
}