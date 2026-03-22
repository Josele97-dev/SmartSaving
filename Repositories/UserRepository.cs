using System.Collections.Generic;
using System.Linq;

namespace SmartSaving.Repositories
{
    public class UserRepository
    {
        private static List<User> users = new List<User>();

        public bool Register(User user)
        {
            if (users.Any(u => u.Email == user.Email))
                return false;
            users.Add(user);
            return true;
        }

        public User GetByEmail(string email)
        {
            return users.FirstOrDefault(u => u.Email == email);
        }
    }
}