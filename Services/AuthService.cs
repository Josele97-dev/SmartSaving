using System.Collections.Generic;
using System.Linq;

public class AuthService : IAuthService
{
    private readonly List<User> _users = new List<User>();

    public User Login(string email, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Email == email && u.PasswordHash == password);
    }

    public User Register(string email, string password)
    {
        if (_users.Any(u => u.Email == email))
            throw new Exception("User already exists");

        var user = new User
        {
            Email = email,
            PasswordHash = password // later: hash this
        };

        _users.Add(user);
        return user;
    }
}