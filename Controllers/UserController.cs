using ChatApp.Services;

namespace ChatApp.Controllers;

public class UserController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    // think about it
}
