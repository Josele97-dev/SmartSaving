using SmartSaving.Repositories;
using System;

namespace SmartSaving.Services
{
    public class AuthService
    {
        private readonly UserRepository userRepository = new UserRepository();

        public bool Register(string email, string password, string firstName, string lastName)
        {
            var user = new User
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName
            };
            return userRepository.Register(user);
        }

        public bool Login(string email, string password)
        {
            var user = userRepository.GetByEmail(email);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}