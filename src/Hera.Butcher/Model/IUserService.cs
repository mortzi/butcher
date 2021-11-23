namespace Hera.Butcher.Model;

public interface IUserService
{
    Task<User> CreateUser(long telegramId);
    Task<User?> FindUser(long telegramId);
}
