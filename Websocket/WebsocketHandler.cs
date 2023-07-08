using System.Net.WebSockets;
using ChatApp.Services;

namespace ChatApp.Websocket;

public class WebsocketHandler
{
    private readonly ConnectionManager _connectionManager;
    private IMessageService _messageService;

    public WebsocketHandler(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public void SetMessageService(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task OnConnected(WebSocket socket, string userId)
    {
        await _connectionManager.AddSocket(userId, socket);
    }

    public async Task OnDisconnected(string userId)
    {
        await _connectionManager.RemoveSocket(userId);
    }

    public async Task ReceiveMessageAsync(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                    cancellationToken: CancellationToken.None);
        _messageService.SetConnectionManager(_connectionManager);
        while (!result.CloseStatus.HasValue)
        {
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
            await _messageService.SendMessage(buffer, result);
            result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                    cancellationToken: CancellationToken.None);
        }

        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed by handler", CancellationToken.None);
    }
}
