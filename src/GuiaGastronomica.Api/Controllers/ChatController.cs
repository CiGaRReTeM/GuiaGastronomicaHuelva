using GuiaGastronomica.Api.Services;
using GuiaGastronomica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GuiaGastronomica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> SendMessage([FromBody] ChatMessageDto message)
    {
        _logger.LogInformation("Received chat message via HTTP: {Message}", message.Message);

        var response = await _chatService.ProcessMessageAsync(message.Message);
        return Ok(response);
    }

    [HttpDelete("history")]
    public IActionResult ClearHistory()
    {
        _chatService.ClearHistory();
        return Ok(new { message = "Chat history cleared successfully" });
    }
}
