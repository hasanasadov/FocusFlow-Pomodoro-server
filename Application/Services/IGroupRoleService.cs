namespace Application.Services;

public interface IGroupRoleService
{
    Task<Result<GroupRoleDto>> GetGroupRoleAsync(int groupRoleId, CancellationToken cancellationToken);
    Task<Result<List<GroupRoleDto>>> GetGroupRolesAsync(int groupId, CancellationToken cancellationToken);
    ValueTask<Result<List<string>>> GetAllPermission();

    Task<Result> CreateGroupRoleAsync(int groupId, CreateGroupRoleDto groupRole, CancellationToken cancellationToken);
    Task<Result> UpdateGroupRoleAsync(UpdateGroupRoleDto groupRole, CancellationToken cancellationToken);
    Task<Result> DeleteGroupRoleAsync(int groupRoleId, CancellationToken cancellationToken);

    Task<Result> AddOrRemovePermissionToGroupRoleAsync(int groupRoleId, List<string> permission, bool isRemove, CancellationToken cancellationToken);
}