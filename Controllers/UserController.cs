using System.Security.Claims;
using ChatApp.Dto;
using ChatApp.Extension;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("add-contact")]
    public async Task<IActionResult> AddContact([FromBody] AddContactRequest request)
    {
        var userName = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;

        var result = await _userService.AddContact(userName, request);

        return result.ToOkResponse();
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userId = HttpContext.User.Claims.First(x => x.Type == "UserId").Value;
        var result = await _userService.GetFriends(userId);

        return result.ToOkResponse();
    }
}
