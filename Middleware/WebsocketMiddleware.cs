using ChatApp.Services;
using ChatApp.Websocket;

namespace ChatApp.Middleware;

public class WebsocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebsocketHandler _websocketHandler;

    public WebsocketMiddleware(RequestDelegate next, WebsocketHandler websocketHandler)
    {
        _next = next;
        _websocketHandler = websocketHandler;
    }

    public async Task InvokeAsync(HttpContext context, IMessageService messageService)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var userId = context.Request.Query["userId"].ToString();
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            _websocketHandler.SetMessageService(messageService);
            await _websocketHandler.OnConnected(websocket, userId);
            await _websocketHandler.ReceiveMessageAsync(websocket);
        }
        else
        {
            await _next(context);
        }
    }
}

public static class WebsocketMiddlewareExtension
{
    public static IApplicationBuilder UseWebsocketHandler(
        this IApplicationBuilder builder)
    {
        var handler = builder.ApplicationServices.GetRequiredService<WebsocketHandler>();
        return builder.UseMiddleware<WebsocketMiddleware>(handler);
    }
}