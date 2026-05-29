using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("/send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        // Broadcast the message to all clients via SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        return Ok();
    }
}