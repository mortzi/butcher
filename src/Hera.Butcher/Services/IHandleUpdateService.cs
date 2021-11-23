using Telegram.Bot.Types;

namespace Hera.Butcher.Services;

public interface IHandleUpdateService
{
    Task EchoAsync(Update update);
}
