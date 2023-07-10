using ChatApp.Data;
using ChatApp.Dto;
using ChatApp.Enum;
using ChatApp.Models;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services;

public interface IChatService
{
    Task<Result<bool>> CreateGroupAsync(CreateGroupRequest request);
    Task<Result<bool>> AddUserToGroupAsync(string groupId, AddUserToGroupRequest request);
    Task<Result<bool>> ChangeThreadState(string userId, ChangeThreadStateRequest request);
    Task<List<ThreadPreviewResponse>> FetchAllThreads(string userId);
}

public class ChatService : IChatService
{
    private readonly AppDbContext _dbContext;

    public ChatService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> AddUserToGroupAsync(string groupId, AddUserToGroupRequest request)
    {
        var group = await _dbContext.Groups.Include(x => x.GroupMemberInfos).FirstOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
        {
            var exception = new ApiException("Group not found");
            return new Result<bool>(exception);
        }

        var existingUsers = group.GroupMemberInfos.Select(x => x.UserId).ToList();

        var usersToAdd = request.UserIds.Where(x => !existingUsers.Contains(x)).ToList();

        var groupMemberInfo = new List<GroupMemberInfo>();

        foreach (var member in usersToAdd)
        {
            groupMemberInfo.Add(new()
            {
                UserId = member
            });
        }

        group.GroupMemberInfos.AddRange(groupMemberInfo);
        _dbContext.Groups.Update(group);

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool>> ChangeThreadState(string userId, ChangeThreadStateRequest request)
    {
        var userExist = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (userExist == null)
        {
            var exception = new ApiException("User not found");
            return new Result<bool>(exception);
        }

        var thread = await _dbContext.Threads.FirstOrDefaultAsync(x => x.UserId == userId && x.ChannelId == request.ChannelId && x.Type == (ChannelType)request.ChannelType);

        if (thread == null)
        {
            var exception = new ApiException("No active thread with given id found");
            return new Result<bool>(exception);
        }

        thread.State = (ChatApp.Enum.ThreadState)request.State;
        _dbContext.Threads.Update(thread);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool>> CreateGroupAsync(CreateGroupRequest request)
    {
        var userExist = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.CreatedByUser);

        if (userExist == null)
        {
            var exception = new ApiException("User not found");
            return new Result<bool>(exception);
        }

        var groupMemberInfo = new List<GroupMemberInfo>();

        foreach (var member in request.UserIds)
        {
            groupMemberInfo.Add(new()
            {
                UserId = member
            });
        }

        groupMemberInfo.Add(new()
        {
            UserId = request.CreatedByUser,
            IsAdmin = true
        });

        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            GroupMemberInfos = groupMemberInfo
        };

        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<ThreadPreviewResponse>> FetchAllThreads(string userId)
    {
        var allThreads = await _dbContext.Threads.Where(x => x.UserId == userId && x.State != Enum.ThreadState.Deleted).ToListAsync();

        var users = allThreads.Where(x => x.Type == ChannelType.User).Select(x => x.ChannelId).ToList();
        var groups = allThreads.Where(x => x.Type == ChannelType.Group).Select(x => x.ChannelId).ToList();

        var userThreads = await _dbContext.Users.Where(x => users.Contains(x.Id)).ToListAsync();
        var groupThreads = await _dbContext.Groups.Where(x => groups.Contains(x.Id)).ToListAsync();

        List<ThreadPreviewResponse> response = new();

        foreach (var thread in userThreads)
        {
            response.Add(new()
            {
                ChannelId = thread.Id,
                DpUrl = thread.DpUrl,
                Description = null,
                Name = thread.UserName,
                ChannelType = allThreads.Find(x => x.ChannelId == thread.Id).Type,
                ThreadState = allThreads.Find(x => x.ChannelId == thread.Id).State,
            });
        }

        foreach (var thread in groupThreads)
        {
            response.Add(new()
            {
                ChannelId = thread.Id,
                DpUrl = thread.DpUrl,
                Description = thread.Description,
                Name = thread.Name,
                ChannelType = allThreads.Find(x => x.ChannelId == thread.Id).Type,
                ThreadState = allThreads.Find(x => x.ChannelId == thread.Id).State,
            });
        }

        return response;
    }
}
