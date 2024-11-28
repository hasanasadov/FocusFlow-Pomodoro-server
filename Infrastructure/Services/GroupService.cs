using Application.Services.CacheService;
using Application.Services.PhotoService;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services;

public sealed class GroupService(AppDbContext dbContext,
                                 IUserContext userContext,
                                 ICacheService cacheService,
                                 ILocalPhotoService localPhotoService) : IGroupService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IUserContext _userContext = userContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILocalPhotoService _localPhotoService = localPhotoService;

    public async Task<Result<GroupDto>> GetGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        Group? group = await _dbContext.Groups
            .Include(x => x.User)
            .Include(group => group.Memberships)
            .Include(group => group.Projects)
            .ThenInclude(project => project.Tasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(group => group.Id == groupId, cancellationToken);

        int tasksCount = await _dbContext.Projects.CountAsync(x => x.GroupId == groupId, cancellationToken);

        if (group is null)
        {
            return Result<GroupDto>.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }
        return Result<GroupDto>.Success(new(group.Id,
                                            group.Name,
                                            group.Description,
                                            group.Photo,
                                            group.Projects.Count,
                                            group.User.UserName ?? "Anonymous",
                                            group.CreatedDate,
                                            group.Memberships.Count,
                                            tasksCount
                                            ));
    }

    public async Task<Result<List<GetAllGroupDto>>> GetAllGroupsAsync(CancellationToken cancellationToken)
    {
        List<GetAllGroupDto> groups = await _dbContext.Groups
            .AsNoTracking()
            .Where(group => group.Memberships.Any(groupMembership => groupMembership.UserId == _userContext.UserId))
            .Select(group => new GetAllGroupDto(group.Id, group.Name, group.Description, group.Photo))
            .ToListAsync(cancellationToken);

        return Result<List<GetAllGroupDto>>.Success(groups);
    }

    public async Task<Result<Dictionary<int, GroupMembership>>> GetGroupMembershipsAsync(CancellationToken cancellationToken)
    {
        Dictionary<int, GroupMembership> groups = await _dbContext.GroupMemberships
            .AsNoTracking()
            .Where(groupMembership => groupMembership.UserId == _userContext.UserId)
            .ToDictionaryAsync(groupMembership => groupMembership.GroupId, cancellationToken);

        return Result<Dictionary<int, GroupMembership>>.Success(groups);
    }

    public async Task<Result<List<GroupMemberDto>>> GetMembersAsync(int groupId, CancellationToken cancellationToken)
    {
        Group? group = await _dbContext.Groups
            .Include(group => group.Memberships)
            .ThenInclude(x => x.User)
            .Include(x => x.Memberships)
            .ThenInclude(x => x.GroupRole)
            .AsSplitQuery()
            .FirstOrDefaultAsync(group => group.Id == groupId && 
                                 group.Memberships.Any(x => x.UserId == _userContext.UserId), cancellationToken);

        if (group is null)
        {
            return Result<List<GroupMemberDto>>.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }

        List<GroupMemberDto> members = group.Memberships
            .Select(groupMembership => new GroupMemberDto(groupMembership.Id,
                                                          groupMembership.User.UserName!,
                                                          groupMembership.User.PictureUrl,
                                                          groupMembership.GroupRoleId,
                                                          groupMembership.GroupRole.Name))
            .ToList();

        return Result<List<GroupMemberDto>>.Success(members);
    }

    public async Task<Result> CreateGroupAsync(CreateGroupDto group, CancellationToken cancellationToken)
    {
        Guid userId = _userContext.UserId;
        var newGroup = Group.CreateGroup(userId, group.Name, group.Description);
        newGroup.Memberships.Add(new() { UserId = userId, GroupRoleId = (int) GroupRoleIds.Admin });

        EntityEntry result = await _dbContext.Groups.AddAsync(newGroup);
        if (result.State == EntityState.Added)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        return Result.Failure(Error.DatabaseError(nameof(Group)));
    }

    public async Task<Result> UpdateGroupAsync(UpdateGroupDto group, CancellationToken cancellationToken)
    {
        Group? groupToUpdate = await _dbContext.Groups
            .Include(group => group.Memberships)
            .FirstOrDefaultAsync(group => group.Id == group.Id, cancellationToken);

        if (groupToUpdate is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }

        groupToUpdate.Name = group.Name;
        groupToUpdate.Description = group.Description;

        EntityEntry result = _dbContext.Groups.Update(groupToUpdate);

        if (result.State == EntityState.Modified)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        return Result.Failure(Error.DatabaseError(nameof(Group)));
    }

    public async Task<Result> DeleteGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var group = await _dbContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group is null)
            {
                return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
            }

            var deleteMembershipsTask = _dbContext.GroupMemberships
                .Where(gm => gm.GroupId == groupId)
                .ExecuteDeleteAsync(cancellationToken);

            var deleteProjectsTask = _dbContext.Projects
                .Where(p => p.Group.Id == groupId)
                .ExecuteDeleteAsync(cancellationToken);

            await Task.WhenAll(deleteMembershipsTask, deleteProjectsTask);

            await _dbContext.Groups
                .Where(g => g.Id == groupId)
                .ExecuteDeleteAsync(cancellationToken);
            //TODO soft delete ?
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.DatabaseError($"Failed to delete group: {ex.Message}"));
        }
    }


    public async Task<Result> AddUserToGroup(string usernameOrEmail, int groupId, CancellationToken cancellationToken)
    {
        AppUser? user = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.GroupMemberships)
            .FirstOrDefaultAsync(user => user.UserName == usernameOrEmail ||
                                 user.Email == usernameOrEmail, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound(AuthErrors.UserNotFound));
        }

        var group = await _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }

        if (user.GroupMemberships.Any(groupMembership => groupMembership.GroupId == groupId))
        {
            return Result.Failure(Error.Conflict(GroupErrors.UserAlreadyInGroup));
        }

        group.AddMember(user.Id, (int)GroupRoleIds.User);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }

    public async Task<Result> RemoveUserFromGroup(string usernameOrEmail, int groupId, CancellationToken cancellationToken)
    {
        AppUser? user = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.GroupMemberships)
            .FirstOrDefaultAsync(user => user.UserName == usernameOrEmail ||
                                 user.Email == usernameOrEmail, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound(AuthErrors.UserNotFound));
        }
        Group? group = await _dbContext.Groups.Include(x => x.Memberships).FirstOrDefaultAsync(group => group.Id == groupId, cancellationToken);
        
        if (group is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }


        bool isDeleted = group.RemoveMember(user.Id);

        if (!isDeleted)
            return Result.Failure(Error.NotFound(GroupErrors.UserNotInGroup));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeUseRoleInGroup(string usernameOrEmail, int groupId, int roleId, CancellationToken cancellationToken)
    {
        AppUser? user = await _dbContext.Users
            .Include(user => user.GroupMemberships)
            .FirstOrDefaultAsync(user => user.UserName == usernameOrEmail ||
                                 user.Email == usernameOrEmail, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound(AuthErrors.UserNotFound));
        }

        GroupMembership? groupMembership = user.GroupMemberships.FirstOrDefault(
            groupMembership => groupMembership.GroupId == groupId && 
            groupMembership.UserId == user.Id);

        if (groupMembership is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.UserNotInGroup));
        }

        GroupRole? role = await _dbContext.GroupRoles.AsNoTracking()
            .FirstOrDefaultAsync(groupRole => groupRole.Id == roleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.RoleNotFound));
        }

        groupMembership.ChangeRole(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<GroupPhotoDto>> UploadGroupPhotoAsync(IFormFile file, int groupId, CancellationToken cancellationToken)
    {
        string filePath = await _localPhotoService.UploadPhoto(file, "group-files");
        await _dbContext.Groups
            .Where(group => group.Id == groupId)
            .ExecuteUpdateAsync(group => group.SetProperty(x => x.Photo, filePath), cancellationToken);
        return Result<GroupPhotoDto>.Success(new(groupId, filePath));
    }
}
