using System.Net;
using ChatApp.Data;
using ChatApp.Dto;
using ChatApp.Models;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services;

public interface IAuthService
{
    Task<Either<AuthResponse, List<ApiException>>> Register(RegisterRequest request);
    Task<Either<AuthResponse, ApiException>> Login(LoginRequest request);
    Task<Result<bool>> ForgotPassword(string email, string newPassword);
    Task<Result<bool>> Logout(string userName);
    Task<Either<TokenResponse, ApiException>> GetToken(string refreshToken);
    Task<Result<bool>> SendOtp(SendOtpRequest request);
    Task<Result<bool>> VerifyOtp(VerifyOtpRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    public AuthService(AppDbContext dbContext, IConfiguration configuration, ITokenService tokenService, IValidator<RegisterRequest> registerValidator)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
        _registerValidator = registerValidator;
    }

    public async Task<Result<bool>> ForgotPassword(string email, string newPassword)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            var exception = new ApiException("User Not Found", HttpStatusCode.NotFound);
            return new Result<bool>(exception);
        }

        var hashedPassword = GenerateHash(newPassword);
        user.Password = hashedPassword;
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Either<AuthResponse, ApiException>> Login(LoginRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);

        if (user == null)
        {
            var exception = new ApiException("User Not Found", HttpStatusCode.NotFound);
            return exception;
        }

        if (!VerifyPassword(request.Password, user.Password))
        {
            var exception = new ApiException("Please enter valid credentials");
            return exception;
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

    public async Task<Result<bool>> Logout(string userName)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userName);

        if (user == null)
        {
            var exception = new ApiException("User Not Found", HttpStatusCode.NotFound);
            return new Result<bool>(exception);
        }

        if (user.RefreshToken == null)
        {
            var alreadyLoggedOut = new ApiException("User already logged out", HttpStatusCode.BadRequest);
            return new Result<bool>(alreadyLoggedOut);
        }

        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(-1);
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Either<AuthResponse, List<ApiException>>> Register(RegisterRequest request)
    {
        List<ApiException> errorMessage = new();
        var validationErrors = await _registerValidator.ValidateAsync(request);

        if (!validationErrors.IsValid)
        {
            foreach (var error in validationErrors.Errors)
            {
                errorMessage.Add(new ApiException(error.ErrorMessage));
            }

            return errorMessage;
        }

        var hashedPassword = GenerateHash(request.Password);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = hashedPassword,
            PhoneNumber = request.PhoneNumber,
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

    public async Task<Either<TokenResponse, ApiException>> GetToken(string refreshToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

        if (user is null)
        {
            var exception = new ApiException("Invalid refresh token", HttpStatusCode.Unauthorized);
            return exception;
        }

        if (user.RefreshToken is null || user.RefreshTokenExpireTime <= DateTime.UtcNow)
        {
            var exception = new ApiException("Refresh token expired or invalid", HttpStatusCode.Unauthorized);
            return exception;
        }

        return new TokenResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user)
        };
    }

    public async Task<Result<bool>> SendOtp(SendOtpRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            var exception = new ApiException("User Not Found", HttpStatusCode.NotFound);
            return new Result<bool>(exception);
        }

        var alreadySentOtp = await _dbContext.OtpStores.FirstOrDefaultAsync(x => x.Email == request.Email && x.ExpiresAt >= DateTime.UtcNow);

        if (alreadySentOtp != null)
        {
            var exception = new ApiException("Please wait for 15 minutes before requesting another otp", HttpStatusCode.BadRequest);
            return new Result<bool>(exception);
        }

        var otp = String.Format("{0:0000}", GenerateOtp());
        var otpStore = new OtpStore
        {
            Otp = otp,
            Email = request.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        await _dbContext.OtpStores.AddAsync(otpStore);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool>> VerifyOtp(VerifyOtpRequest request)
    {
        var sendOtp = await _dbContext.OtpStores.FirstOrDefaultAsync(x => x.Email == request.Email && x.ExpiresAt >= DateTime.UtcNow);

        if (sendOtp == null || sendOtp.Otp != request.Otp)
        {
            var exception = new ApiException("Invalid Otp, pls type the valid otp", HttpStatusCode.BadRequest);
            return new Result<bool>(exception);
        }

        return true;
    }

    private string GenerateHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private int GenerateOtp()
    {
        Random _random = new Random();
        return _random.Next(0, 9999);
    }
}
