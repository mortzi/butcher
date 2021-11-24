using Microsoft.AspNetCore.Mvc;
using Hera.Butcher.Services;
using Telegram.Bot.Types;

namespace Hera.Butcher.Controllers;

public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
                                          [FromBody] Update update)
    {
        await handleUpdateService.EchoAsync(update);
        return Ok();
    }
}
