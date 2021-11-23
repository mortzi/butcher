using Hera.Butcher.Model;
using Microsoft.EntityFrameworkCore;

namespace Hera.Butcher.Services;

public class TradeService : ITradeService
{
    private readonly TradeDbContext _dbContext;

    public TradeService(TradeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Trade>> GetTrades(Guid userId)
    {
        return await _dbContext.Trades
            .Where(trade => trade.UserId == userId)
            .ToListAsync();
    }

    public async Task<Trade> Submit(Guid userId, Trade trade)
    {
        var oldTrade = await _dbContext.Trades.FirstOrDefaultAsync(t => t.Id == trade.Id);

        if (oldTrade is not null)
            _dbContext.Trades.Remove(oldTrade);

        _dbContext.Trades.Add(trade);


        await _dbContext.SaveChangesAsync();

        return trade;
    }
}
