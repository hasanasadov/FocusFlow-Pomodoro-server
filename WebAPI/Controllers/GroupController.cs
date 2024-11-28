using Application.Services.AuthService.Local;
using System.Runtime.CompilerServices;
using WebAPI.Attributes;

namespace WebAPI.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GroupController : BaseController
{
    private readonly IGroupService _groupService;
    private readonly ILocalAuthService _authService;

    public GroupController(IGroupService groupService, ILocalAuthService authService)
    {
        _groupService = groupService;
        _authService = authService;
    }

    [HttpGet("{groupId}")]
    [GroupRolePermission(CheckRoute = true)]
    public async Task<IActionResult> GetGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.GetGroupAsync(groupId, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroupsAsync(CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.GetAllGroupsAsync(cancellationToken));
    }

    [HttpGet("memberships")]
    public async Task<IActionResult> GetGroupMembershipsAsync(CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.GetGroupMembershipsAsync(cancellationToken));
    }

    [HttpPatch]
    public async Task<IActionResult> UploadGroupPhotoAsync(IFormFile file, int groupId,CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.UploadGroupPhotoAsync(file, groupId, cancellationToken));
    }

    [HttpGet("{groupId}/members")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.GetGroupMembers], CheckRoute = true)]
    public async Task<IActionResult> GetMembersAsync(int groupId, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.GetMembersAsync(groupId, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupAsync(CreateGroupDto group, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.CreateGroupAsync(group, cancellationToken));
    }

    [HttpPost("{groupId}/add-user")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.AddUserToGroup], CheckRoute = true)]
    public async Task<IActionResult> AddUserToGroup(int groupId, string usernameOrEmail, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.AddUserToGroup(usernameOrEmail, groupId, cancellationToken));
    }

    [HttpPost("{groupId}/remove-user")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.RemoveUserFromGroup], CheckRoute = true)]
    public async Task<IActionResult> RemoveUserFromGroup(string usernameOrEmail, int groupId, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.RemoveUserFromGroup(usernameOrEmail, groupId, cancellationToken));
    }

    [HttpPost("{groupId}/change-role")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.ChangeUserRoleInGroup], CheckRoute = true)]
    public async Task<IActionResult> ChangeUserRoleInGroup(string usernameOrEmail, int groupId, int roleId, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.ChangeUseRoleInGroup(usernameOrEmail, groupId, roleId, cancellationToken));
    }

    [HttpPut("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.UpdateGroup], CheckRoute = true)]
    public async Task<IActionResult> UpdateGroupAsync(int groupId, UpdateGroupDto group, CancellationToken cancellationToken)
    {
        group.Id = groupId;
        return FromResult(await _groupService.UpdateGroupAsync(group, cancellationToken));
    }

    [HttpDelete("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.DeleteGroup], CheckRoute = true)]
    public async Task<IActionResult> DeleteGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        return FromResult(await _groupService.DeleteGroupAsync(groupId, cancellationToken));
    }
}
