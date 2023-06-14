using ChatApp.Services;

namespace ChatApp.Controllers;

public class ChatController
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
    // creating group
    // add user to group
    // get active chats for user
}
