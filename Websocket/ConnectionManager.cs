using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatApp.Websocket;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public WebSocket GetSocketById(string userId)
    {
        _sockets.TryGetValue(userId, out WebSocket socket);
        return socket;
    }

    public async Task AddSocket(string userId, WebSocket socket)
    {
        _sockets.AddOrUpdate(userId, socket, (key, oldSocket) =>
        {
            return _sockets[key] = socket;
        });

        await Task.CompletedTask;
    }

    public async Task RemoveSocket(string userId)
    {
        WebSocket socket;
        _sockets.TryRemove(userId, out socket);

        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by connection manager", CancellationToken.None);
    }
}
