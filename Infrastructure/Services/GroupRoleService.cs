using Application.Constants;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services;

public sealed class GroupRoleService(AppDbContext dbContext) : IGroupRoleService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<GroupRoleDto>> GetGroupRoleAsync(int groupRoleId, CancellationToken cancellationToken)
    {
        GroupRole? groupRole = await _dbContext.GroupRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(groupRole => groupRole.Id == groupRoleId ||
                                 groupRole.IsDefault, cancellationToken);

        if (groupRole is null)
        {
            return Result<GroupRoleDto>.Failure(Error.NotFound(GroupRoleErrors.GroupRoleNotFound));
        }

        return Result<GroupRoleDto>.Success(new(groupRole.Id, groupRole.Name, groupRole.Permissions));
    }

    public ValueTask<Result<List<string>>> GetAllPermission() => ValueTask
        .FromResult(Result<List<string>>
            .Success(GroupRolePermissionConstants
                .GetAllPermissions()
                .Keys
                .ToList()));

    public async Task<Result<List<GroupRoleDto>>> GetGroupRolesAsync(int groupId, CancellationToken cancellationToken)
    {
        List<GroupRole> groupRole = await _dbContext.GroupRoles
            .AsNoTracking()
            .Where(x => x.GroupId == groupId || x.IsDefault)
            .ToListAsync(cancellationToken);

        return Result<List<GroupRoleDto>>.Success(groupRole
            .Select(x => new GroupRoleDto(x.Id, x.Name, x.Permissions))
            .ToList());
    }

    public async Task<Result> CreateGroupRoleAsync(int groupId, CreateGroupRoleDto groupRole, CancellationToken cancellationToken)
    {
        Group? group = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }

        GroupRole newGroupRole = GroupRole.Create(groupRole.Name, groupId, groupRole.Permissions);

        EntityEntry result = await _dbContext.GroupRoles.AddAsync(newGroupRole, cancellationToken);

        if (result.State == EntityState.Added)
        {
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        return Result.Failure(Error.DatabaseError(nameof(GroupRole)));
    }

    public async Task<Result> UpdateGroupRoleAsync(UpdateGroupRoleDto groupRole, CancellationToken cancellationToken)
    {
        GroupRole? existingGroupRole = await _dbContext.GroupRoles
            .FirstOrDefaultAsync(x => x.Id == groupRole.GroupRoleId && !x.IsDefault, cancellationToken);

        if (existingGroupRole is null)
        {
            return Result.Failure(Error.NotFound(GroupRoleErrors.GroupRoleNotFound));
        }

        existingGroupRole.Update(groupRole.Name, groupRole.Permissions);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteGroupRoleAsync(int groupRoleId, CancellationToken cancellationToken)
    {
        GroupRole? groupRole = await _dbContext.GroupRoles
            .FirstOrDefaultAsync(x => x.Id == groupRoleId && !x.IsDefault, cancellationToken);

        if (groupRole is null)
        {
            return Result.Failure(Error.NotFound(GroupRoleErrors.GroupRoleNotFound));
        }

        groupRole.Delete();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AddOrRemovePermissionToGroupRoleAsync(int groupRoleId, List<string> permission, bool isRemove, CancellationToken cancellationToken)
    {
        GroupRole? groupRole = await _dbContext.GroupRoles
            .FirstOrDefaultAsync(x => x.Id == groupRoleId, cancellationToken);

        if (groupRole is null)
        {
            return Result.Failure(Error.NotFound(GroupRoleErrors.GroupRoleNotFound));
        }
        if (isRemove)
        {
            groupRole.RemovePermission(permission);
        }
        else
        {
            groupRole.AddPermission(permission);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

}