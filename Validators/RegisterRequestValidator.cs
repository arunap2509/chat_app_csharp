using ChatApp.Data;
using ChatApp.Dto;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address");
        RuleFor(x => x.Password).NotEmpty().Must(x => x.Length > 8).WithMessage("{PropertyName} should be minimum of 8 characters");
        RuleFor(x => x.PhoneNumber).NotEmpty().Must(x => x.Length == 10).WithMessage("{PropertyName} is not valid");

        RuleFor(x => x.Email).MustAsync(async (email, cancallationToken) => await ValidateEmailAlreadyExist(email, dbContext)).WithMessage("Email already exists");

        RuleFor(x => x.UserName).MustAsync(async (userName, cancallationToken) => await ValidateUserNameAlreadyExist(userName, dbContext)).WithMessage("Username already exists");
    }

    private async Task<bool> ValidateEmailAlreadyExist(string email, AppDbContext dbContext)
    {
        return !await dbContext.Users.AnyAsync(x => x.Email == email);
    }

    private async Task<bool> ValidateUserNameAlreadyExist(string userName, AppDbContext dbContext)
    {
        return !await dbContext.Users.AnyAsync(x => x.UserName == userName);
    }
}
