using ChatApp.Data;
using ChatApp.Dto;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
    Task ForgotPassword(string email, string newPassword);
    Task Logout(string userName);
    Task<TokenResponse> GetToken(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext dbContext, IConfiguration configuration, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task ForgotPassword(string email, string newPassword)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            // throw
        }

        var hashedPassword = GenerateHash(newPassword);
        user.Password = hashedPassword;
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);

        if (user == null || VerifyPassword(request.Password, user.Password))
        {
            // throw an error
        }

        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(1);
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = user.RefreshToken
        };
    }

    public async Task Logout(string userName)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userName);

        if (user == null || user.RefreshToken == null)
        {
            // throw an error
        }

        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(-1);
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var isEmailAlreadyExist = await _dbContext.Users.AnyAsync(x => x.Email == request.Email);
        var isUserNameAlreadyExist = await _dbContext.Users.AnyAsync(x => x.UserName == request.UserName);
        List<string> errorMessage = new();

        if (isEmailAlreadyExist)
        {
            errorMessage.Add("Email already exists");
        }

        if (isUserNameAlreadyExist)
        {
            errorMessage.Add("Username already exists");
        }

        if (errorMessage.Any())
        {
            throw new Exception();
        }

        var hashedPassword = GenerateHash(request.Password);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(1);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse()
        {
            UserId = user.Id,
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = user.RefreshToken
        };

        return response;
    }

    public async Task<TokenResponse> GetToken(string refreshToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

        if (user is null || user.RefreshToken is null || user.RefreshTokenExpireTime <= DateTime.UtcNow)
        {
            // throw unauthorized request
        }

        return new TokenResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user)
        };
    }

    private string GenerateHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
