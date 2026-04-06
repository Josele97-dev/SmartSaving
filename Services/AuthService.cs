using System;
using System.Threading.Tasks;
using SmartSaving.Models;
using SmartSaving.Repositories;

namespace SmartSaving.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        public AuthService(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return null;

            // Verify the password against the stored BCrypt hash
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isValid ? user : null;
        }

        public async Task<User?> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            // Check if a user with this email already exists
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return null;

            // Hash the password with BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = email,
                Email = email,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                CreationDate = DateTime.Now
            };

            bool registered = await _userRepository.RegisterAsync(user);

            if (!registered)
                return null;

            // Create a default account for the new user
            var defaultAccount = new Account
            {
                UserId = user.Id,
                AccountName = "Main Account",
                CurrentBalance = 0
            };

            await _accountRepository.AddAsync(defaultAccount);

            // Reload user with accounts included
            return await _userRepository.GetByIdAsync(user.Id);
        }
    }
}
