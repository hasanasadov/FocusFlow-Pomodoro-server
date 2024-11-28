using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IGroupService
{
    Task<Result<GroupDto>> GetGroupAsync(int groupId, CancellationToken cancellationToken);
    Task<Result<List<GetAllGroupDto>>> GetAllGroupsAsync(CancellationToken cancellationToken);
    Task<Result<Dictionary<int, GroupMembership>>> GetGroupMembershipsAsync(CancellationToken cancellationToken);

    Task<Result> CreateGroupAsync(CreateGroupDto group, CancellationToken cancellationToken);
    Task<Result<GroupPhotoDto>> UploadGroupPhotoAsync(IFormFile file, int groupId, CancellationToken cancellationToken);
    Task<Result> DeleteGroupAsync(int groupId, CancellationToken cancellationToken);
    Task<Result> UpdateGroupAsync(UpdateGroupDto group, CancellationToken cancellationToken);

    Task<Result> AddUserToGroup(string usernameOrEmail, int groupId, CancellationToken cancellationToken);
    Task<Result> RemoveUserFromGroup(string usernameOrEmail, int groupId, CancellationToken cancellationToken);
    Task<Result> ChangeUseRoleInGroup(string usernameOrEmail, int groupId, int roleId, CancellationToken cancellationToken);
    Task<Result<List<GroupMemberDto>>> GetMembersAsync(int groupId, CancellationToken cancellationToken);
}