using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatApp.Data;
using ChatApp.Dto;
using ChatApp.Enum;
using Microsoft.EntityFrameworkCore;
using ChatApp.Websocket;

namespace ChatApp.Services;

public interface IMessageService
{
    void SetConnectionManager(ConnectionManager connectionManager);
    Task SendMessage(byte[] buffer, WebSocketReceiveResult receiveResult);
}

public class MessageService : IMessageService
{
    private readonly AppDbContext _dbContext;
    private ConnectionManager _connectionManager;

    public MessageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SetConnectionManager(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task SendMessage(byte[] buffer, WebSocketReceiveResult receiveResult)
    {
        var model = DeserializeData(buffer, receiveResult.Count);
        await SendMessageAsync(model, receiveResult);
    }

    private SenderMessage DeserializeData(byte[] buffer, int bufferCount)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, bufferCount);
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<SenderMessage>(message, options);
        return model;
    }

    private async Task HandleOneToOneMessage(WebSocketReceiveResult result, ReceiverMessage response)
    {
        var respondTo = _connectionManager.GetSocketById(response.ChannelId);
        await SendMessageAsync(result, respondTo, response);
    }

    private async Task HandleGroupMessage(WebSocketReceiveResult result, SenderMessage model)
    {
        var groupMembers = await _dbContext.Groups.Include(x => x.GroupMemberInfos).Where(x => x.Id == model.ChannelId).SelectMany(x => x.GroupMemberInfos.Select(x => x.UserId).ToList()).ToListAsync();

        foreach (var member in groupMembers)
        {
            var respondTo = _connectionManager.GetSocketById(member);
            if (respondTo == null)
            {
                continue;
            }

            var response = new ReceiverMessage
            {
                Message = model.Message,
                ChannelId = model.ChannelId
            };
            await SendMessageAsync(result, respondTo, response);
        }
    }

    private async Task SendMessageAsync(SenderMessage model, WebSocketReceiveResult result)
    {
        if (model.ChannelType == (short)ChannelType.User)
        {
            var response = new ReceiverMessage
            {
                Message = model.Message,
                ChannelId = model.ChannelId
            };
            await HandleOneToOneMessage(result, response);
        }
        else if (model.ChannelType == (short)ChannelType.Group)
        {
            await HandleGroupMessage(result, model);
        }
    }

    private async Task SendMessageAsync(WebSocketReceiveResult result, WebSocket socket, ReceiverMessage response)
    {
        var serializedData = JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(serializedData);
        var bytesCount = Encoding.UTF8.GetByteCount(serializedData);
        await socket.SendAsync(
                        new ArraySegment<byte>(bytes, 0, bytesCount),
                        result.MessageType,
                        result.EndOfMessage,
                        CancellationToken.None);
    }
}
