namespace Hera.Butcher.Model;

public class Trade
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public DateTimeOffset Date { get; set; }
    public string Market { get; set; } = "BTCUSDT";
    public TradeDirection Type { get; set; } = TradeDirection.None;
    public decimal EntryPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal StopLoss { get; set; }
    public decimal RiskRewardRatio { get; set; }

    public decimal TakeProfit { get; set; }
    public decimal Fee { get; set; }
    public TradeResultType Result { get; set; } = TradeResultType.None;
    public decimal NetProfit { get; set; }
    public decimal AccountGrowth { get; set; }
}

public enum TradeDirection
{
    None = 0,
    Buy = 1,
    Sell = 2
}

public enum TradeResultType
{
    None = 0,
    Tp = 1,
    Sl = 2,
}

