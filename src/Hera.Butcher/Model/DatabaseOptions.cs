using Microsoft.Extensions.Options;

namespace Hera.Butcher.Model;

public class DatabaseOptions : IOptions<DatabaseOptions>
{
    public DatabaseOptions Value => this;

    public string Name { get; set; } = default!;
}
