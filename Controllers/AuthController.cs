using ChatApp.Services;

namespace ChatApp.Controllers;

public class AuthController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    // register 
    // login
}
