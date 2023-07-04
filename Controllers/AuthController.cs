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
        var result = await _authService.Register(request);

        return result.Match<IActionResult>(
            (error) =>
            {
                List<string> errors = new();
                foreach (var exp in error)
                {
                    errors.Add(exp.Message);
                }
                return BadRequest(new { Error = errors });
            },
            (response) =>
            {
                return Ok(response);
            }
        );
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.Login(request);
        return result.Match<IActionResult>((error) =>
        {
            return BadRequest(new { Error = new string[] { error.Message } });
        }, response =>
        {
            return Ok(response);
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userName = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        var result = await _authService.Logout(userName);
        return result.Match<IActionResult>(ok =>
        {
            return Ok();
        }, error =>
        {
            if (error is ApiException exception)
            {
                return StatusCode((int)exception.Code, new { error = new string[] { exception.Message } });
            }

            return BadRequest();
        });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPassword(request.Email, request.NewPassword);
        return result.Match<IActionResult>(response =>
        {
            return Ok();
        }, error =>
        {
            return BadRequest(new { Error = new string[] { error.Message } });
        });
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken()
    {
        var refreshToken = HttpContext.Request.Headers["X-Refresh-Token"].ToString();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest();
        }

        var result = await _authService.GetToken(refreshToken);
        return result.Match<IActionResult>((exception) =>
        {
            return StatusCode((int)exception.Code, new { error = new string[] { exception.Message } });
        }, response =>
        {
            return Ok(response);
        });
    }

    [HttpPost("send-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        var result = await _authService.SendOtp(request);
        return result.Match<IActionResult>(response =>
        {
            return Ok();
        }, error =>
        {
            return BadRequest(new { Error = new string[] { error.Message } });
        });
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VeifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtp(request);
        return result.Match<IActionResult>(response =>
        {
            return Ok();
        }, error =>
        {
            return BadRequest(new { Error = new string[] { error.Message } });
        });
    }
}
