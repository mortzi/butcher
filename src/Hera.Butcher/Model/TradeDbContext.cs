using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hera.Butcher.Model;

public class TradeDbContext : DbContext
{
    public DbSet<Trade> Trades { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;

    public string DbPath { get; }

    public TradeDbContext(IOptions<DatabaseOptions> options)
    {
        DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            options.Value.Name);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

}
