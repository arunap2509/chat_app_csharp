using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Data;
using ChatApp.Dto;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Services;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
    Task ForgotPassword(string userId, string newPassword);
    Task Logout(string userId);
    Task<TokenResponse> GetToken(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public Task ForgotPassword(string userId, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);

        if (user == null || VerifyPassword(request.Password, user.Password))
        {
            // throw an error
        }

        return new AuthResponse
        {
            UserId = user.Id,
            AccessToken = GenerateToken(user),
            RefreshToken = GenerateRefreshToken()
        };
    }

    public async Task Logout(string userId)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            // throw an error
        }

        user.RefreshToken = null;
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
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var accessToken = GenerateToken(user);
        var refreshToken = GenerateRefreshToken();

        var response = new AuthResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return response;
    }

    public Task<TokenResponse> GetToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    private string GenerateHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:RefreshTokenKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
