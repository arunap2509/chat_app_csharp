using ChatApp.Data;
using ChatApp.Dto;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services;

public interface IUserService
{
    Task<Result<bool>> AddContact(string userName, AddContactRequest request);
    Task<Result<List<GetFriendsResponse>>> GetFriends(string userId);
}

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> AddContact(string userName, AddContactRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);

        if (user == null)
        {
            var exception = new ApiException("User not found");
            return new Result<bool>(exception);
        }

        var friendsIds = await _dbContext.Users.Where(x => request.PhoneNumbers.Contains(x.PhoneNumber)).Select(x => x.Id).ToListAsync();

        if (!friendsIds.Any())
        {
            return true;
        }

        user.FriendsIds.AddRange(friendsIds);
        _dbContext.Users.Update(user);

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<List<GetFriendsResponse>>> GetFriends(string userId)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var exception = new ApiException("User not found");
            return new Result<List<GetFriendsResponse>>(exception);
        }

        var response = await _dbContext.Users.Where(x => user.FriendsIds.Contains(x.Id))
        .Select(x => new GetFriendsResponse
        {
            UserId = x.Id,
            UserName = x.UserName,
            DpUrl = x.DpUrl,
            PhoneNumber = x.PhoneNumber
        }).ToListAsync();

        return response;
    }
}
