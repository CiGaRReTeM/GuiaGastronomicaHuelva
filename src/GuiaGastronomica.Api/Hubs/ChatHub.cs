using GuiaGastronomica.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace GuiaGastronomica.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async Task SendMessage(string message)
    {
        try
        {
            _logger.LogInformation("Received message from client: {Message}", message);

            // Procesar el mensaje con el servicio de chat
            var response = await _chatService.ProcessMessageAsync(message);

            // Enviar respuesta al cliente
            await Clients.Caller.SendAsync("ReceiveMessage", response.Response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendMessage: {ErrorMessage}", ex.Message);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Lo siento, ocurrió un error: {ex.Message}");
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await Clients.Caller.SendAsync("ReceiveMessage", "¡Hola! Soy tu asistente de gastronomía en Huelva. ¿En qué puedo ayudarte hoy?");
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
