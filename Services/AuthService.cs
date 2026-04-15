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
        private readonly ICategoryRepository _categoryRepository;

        public AuthService(IUserRepository userRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
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
                CreationDate = DateTime.UtcNow
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
            var defaultCategories = new List<Category>
    {
        new Category { AccountId = defaultAccount.Id, Title = "Sueldo", Type = TransactionType.Income },
        new Category { AccountId = defaultAccount.Id, Title = "Rentas", Type = TransactionType.Income },
        new Category { AccountId = defaultAccount.Id, Title = "Comida", Type = TransactionType.Expense },
        new Category { AccountId = defaultAccount.Id, Title = "Transporte", Type = TransactionType.Expense },
        new Category { AccountId = defaultAccount.Id, Title = "Entretenimiento", Type = TransactionType.Expense }
    };

            foreach (var category in defaultCategories)
            {
                await _categoryRepository.AddAsync(category);
            }

            // Reload user with accounts included
            return await _userRepository.GetByIdAsync(user.Id);
        }
    }
}
