using ChatApp.Dto;
using ChatApp.Extension;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("group/create")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
    {
        var result = await _chatService.CreateGroupAsync(request);
        return result.ToOkResponse();
    }

    [HttpPost("group/{groupId}/users")]
    public async Task<IActionResult> AddUserToGroup([FromRoute] string groupId, [FromBody] AddUserToGroupRequest request)
    {
        var result = await _chatService.AddUserToGroupAsync(groupId, request);
        return result.ToOkResponse();
    }

    [HttpPost("change-thread-state")]
    public async Task<IActionResult> ChangeThreadState([FromBody] ChangeThreadStateRequest request)
    {
        var userId = HttpContext.User.Claims.First(x => x.Type == "UserId").Value;
        var result = await _chatService.ChangeThreadState(userId, request);
        return result.ToOkResponse();
    }
}
