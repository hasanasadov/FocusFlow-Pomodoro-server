using WebAPI.Attributes;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
public sealed class GroupRoleController(
    IGroupRoleService groupRoleService) : BaseController
{
    private readonly IGroupRoleService _groupRoleService = groupRoleService;

    [HttpGet("{groupId}/{groupRoleId}")]
    [GroupRolePermission(Permissions = [], CheckRoute = true)]
    public async Task<IActionResult> GetGroupRoleByIdAsync(
        [FromRoute] int groupId,
        [FromRoute] int groupRoleId,
        CancellationToken cancellationToken)
    {
        var groupRoles = await _groupRoleService.GetGroupRoleAsync(groupRoleId, cancellationToken);
        return FromResult(groupRoles);
    }

    [HttpGet("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.AllGroupRolePermissions], CheckRoute = true)]
    public async Task<IActionResult> GetGroupRolesAsync(
        [FromRoute] int groupId,
        CancellationToken cancellationToken)
    {
        var groupRoles = await _groupRoleService.GetGroupRolesAsync(groupId, cancellationToken);
        return FromResult(groupRoles);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetAllPermissionAsync(CancellationToken cancellationToken)
    {
        var permissions = await _groupRoleService.GetAllPermission();
        return FromResult(permissions);
    }

    [HttpPost("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.CreateGroupRole], CheckRoute = true)]
    public async Task<IActionResult> CreateGroupRoleAsync(
        [FromRoute] int groupId,
        [FromBody] CreateGroupRoleDto groupRole,
        CancellationToken cancellationToken)
    {
        var result = await _groupRoleService.CreateGroupRoleAsync(groupId, groupRole, cancellationToken);
        return FromResult(result);
    }

    [HttpPut("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.UpdateGroupRole], CheckRoute = true)]
    public async Task<IActionResult> UpdateGroupRoleAsync(
        [FromRoute] int groupId,
        [FromBody] UpdateGroupRoleDto groupRole,
        CancellationToken cancellationToken)
    {
        var result = await _groupRoleService.UpdateGroupRoleAsync(groupRole, cancellationToken);
        return FromResult(result);
    }

    [HttpDelete("{groupId}/{groupRoleId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.DeleteGroupRole], CheckRoute = true)]
    public async Task<IActionResult> DeleteGroupRoleAsync(
        [FromRoute] int groupId,
        [FromRoute] int groupRoleId,
        CancellationToken cancellationToken)
    {
        var result = await _groupRoleService.DeleteGroupRoleAsync(groupRoleId, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/permissions/{groupRoleId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.AddPermissionToGroupRole], CheckRoute = true)]
    public async Task<IActionResult> AddPermissionToGroupRoleAsync(
        [FromRoute] int groupId,
        [FromRoute] int groupRoleId,
        [FromBody] List<string> permission,
        CancellationToken cancellationToken)
    {
        var result = await _groupRoleService.AddOrRemovePermissionToGroupRoleAsync(groupRoleId, permission, false, cancellationToken);
        return FromResult(result);
    }

    [HttpDelete("{groupId}/permissions/{groupRoleId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.RemovePermissionFromGroupRole], CheckRoute = true)]
    public async Task<IActionResult> RemovePermissionFromGroupRoleAsync(
        [FromRoute] int groupId,
        [FromRoute] int groupRoleId,
        [FromBody] List<string> permission,
        CancellationToken cancellationToken)
    {
        var result = await _groupRoleService.AddOrRemovePermissionToGroupRoleAsync(groupRoleId, permission, true, cancellationToken);
        return FromResult(result);
    }
}
