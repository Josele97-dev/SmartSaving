public interface IAuthInterface
{
    User Login(string email, string password);
    User Register(string email, string password);
}