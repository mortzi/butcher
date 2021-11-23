using Hera.Butcher.Model;
using Microsoft.EntityFrameworkCore;

namespace Hera.Butcher.Services;

public class UserService : IUserService
{
    private readonly TradeDbContext _dbContext;

    public UserService(TradeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(long telegramId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            TelegramId = telegramId
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> FindUser(long telegramId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }
}
