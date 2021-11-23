using Hera.Butcher.Model;
using Hera.Butcher.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Hera.Butcher;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<ConfigureWebhook>();

        services.AddOptions<BotConfiguration>()
            .Bind(_configuration.GetSection("BotConfiguration"));

        services.AddOptions<DatabaseOptions>()
            .Bind(_configuration.GetSection("Database"));

        services.AddDbContext<TradeDbContext>();

        services
            .AddHttpClient("butcherwebhook")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var token = sp.GetRequiredService<IOptions<BotConfiguration>>().Value.BotToken;
                return new TelegramBotClient(token, httpClient);
            });

        services.AddScoped<HandleUpdateService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITradeService, TradeService>();

        services.AddControllers()
                .AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            var token = app.ApplicationServices
                .GetRequiredService<IOptions<BotConfiguration>>().Value.BotToken;

            endpoints.MapControllerRoute(name: "butcherwebhook",
                                         pattern: $"bot/{token}",
                                         new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }
}
