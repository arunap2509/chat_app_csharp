using System.Security.Claims;
using ChatApp.Dto;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.Register(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userName = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        await _authService.Logout(userName);
        return Ok();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await _authService.ForgotPassword(request.Email, request.NewPassword);
        return Ok();
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken()
    {
        var refreshToken = HttpContext.Request.Headers["X-Refresh-Token"].ToString();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest();
        }

        var response = await _authService.GetToken(refreshToken);
        return Ok(response);
    }
}
