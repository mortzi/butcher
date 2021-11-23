using Hera.Butcher.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hera.Butcher.Services;

public class HandleUpdateService : IHandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITradeService _tradeService;
    private readonly IUserService _userService;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(
        ITelegramBotClient botClient,
        ILogger<HandleUpdateService> logger,
        ITradeService tradeService,
        IUserService userService)
    {
        _botClient = botClient;
        _tradeService = tradeService;
        _userService = userService;
        _logger = logger;
    }

    public async Task EchoAsync(Update update)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            //UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage!),
            //UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
            //UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery!),
            //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
            UpdateType.Message => BotOnMessageReceived(update.Message!),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }

    private async Task BotOnMessageReceived(Message message)
    {
        _logger.LogInformation("Receive message type: {messageType}", message.Type);
        if (message.Type != MessageType.Text)
            return;

        var userId = await GetUserId(message);

        var action = message.Text!.Split(' ')[0] switch
        {
            "/history" => SendHistory(message),
            "/trade" => StartTrade(message),
            _ when DeserializeMessage(message.Text!) is { } trade => ContinueTrade(message.Chat.Id, userId, trade),
            _ => Usage(message)
        };

        Message? sentMessage = await action;

        if (sentMessage is not null)
            _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);


        Trade? DeserializeMessage(string serialized)
        {
            try
            {
                return JsonConvert.DeserializeObject<Trade>(serialized);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on deserializing message to Trade");
                return null;
            }
        }
    }

    private async Task<Message?> ContinueTrade(long chatId, Guid userId, Trade trade)
    {
        await _botClient.SendChatActionAsync(chatId, ChatAction.Typing);

        if (userId != trade.UserId)
        {
            return await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Trade UserId does not match the sender User");
        }

        await _tradeService.Submit(userId, trade);

        _logger.LogInformation("Trade status: {TradeStatus}", trade.AsJson());

        return await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: trade.AsJson());
    }

    private async Task<Message?> StartTrade(Message message)
    {
        await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

        var tradeId = Guid.NewGuid();
        var userId = await GetUserId(message);

        if (userId == default)
        {
            var user = await _userService.CreateUser(message.From!.Id);
            userId = user.Id;
        }

        var trade = new Trade
        {
            Id = tradeId,
            UserId = userId
        };

        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: trade.AsJson());
    }

    private async Task<Guid> GetUserId(Message message)
    {
        var user = await _userService.FindUser(message.From!.Id);

        if (user is null)
            return default;

        return user.Id;
    }

    private async Task<Message?> Usage(Message message)
    {
        const string usage = "Usage:\n" +
                             "/trade   - start a trade\n" +
                             "/history - see trade history\n";

        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: usage,
            replyMarkup: new ReplyKeyboardRemove());
    }


    private async Task<Message?> SendHistory(Message message)
    {
        await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

        var userId = await GetUserId(message);

        if (userId == default)
        {
            return await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "There is no history");
        }

        var trades = await _tradeService.GetTrades(userId);


        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: trades.AsJson());
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
}

public static class TradeJsonExtensions
{
    private static JsonSerializerSettings GetSettings()
    {
        return new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            Converters =
            {
                new StringEnumConverter()
            }
        };
    }

    public static string AsJson(this Trade trade)
    {
        return JsonConvert.SerializeObject(trade, GetSettings());
    }

    public static string AsJson(this IEnumerable<Trade> trades)
    {
        return JsonConvert.SerializeObject(trades, GetSettings());
    }
}
