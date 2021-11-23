namespace Hera.Butcher.Model;

public interface ITradeService
{
    Task<IEnumerable<Trade>> GetTrades(Guid userId);

    Task<Trade> Submit(Guid userId, Trade trade);
}
