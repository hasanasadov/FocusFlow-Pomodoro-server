using WebAPI.Attributes;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{groupId}/{projectId}")]
    [GroupRolePermission(Permissions = [], CheckRoute = true)]
    public async Task<IActionResult> GetProjects(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetProjectAsync(new(groupId, projectId), cancellationToken);
        return FromResult(projects);
    }

    [HttpGet("{groupId}/all")]
    [GroupRolePermission(Permissions = [], CheckRoute = true)]
    public async Task<IActionResult> GetAllProjectsAsync(
        [FromRoute] int groupId,
        CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetAllProjectsAsync(groupId, cancellationToken);
        return FromResult(projects);
    }

    [HttpPost("{groupId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.CreateProject], CheckRoute = true)]
    public async Task<IActionResult> CreateProject(
        [FromRoute] int groupId,
        [FromBody] CreateProjectDto request,
        CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateProjectAsync(groupId, request, cancellationToken);
        return FromResult(project);
    }

    [HttpPut("{groupId}/{projectId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.UpdateProject], CheckRoute = true)]
    public async Task<IActionResult> UpdateProject(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromBody] UpdateProjectDto request,
        CancellationToken cancellationToken)
    {
        var project = await _projectService.UpdateProjectAsync(groupId, request, cancellationToken);
        return FromResult(project);
    }

    [HttpDelete("{groupId}/{projectId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.DeleteProject], CheckRoute = true)]
    public async Task<IActionResult> DeleteProject(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.DeleteProjectAsync(new(groupId, projectId), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/{projectId}/assign")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.AddUserToProject], CheckRoute = true)]
    public async Task<IActionResult> AssignProject(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromQuery] string usernameOrEmail,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.AddOrRemoveUserToProject(usernameOrEmail, new(groupId, projectId), false, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/{projectId}/unassign")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.RemoveUserFromProject], CheckRoute = true)]
    public async Task<IActionResult> UnAssignProject(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromQuery] string usernameOrEmail,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.AddOrRemoveUserToProject(usernameOrEmail, new(groupId, projectId), true, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/{projectId}/tasks")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.CreateTask], CheckRoute = true)]
    public async Task<IActionResult> AddTask(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromBody] CreateTaskDto request,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.AddTaskToProject(new(groupId, projectId), request, cancellationToken);
        return FromResult(result);
    }

    [HttpPut("{groupId}/{projectId}/tasks")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.UpdateTask], CheckRoute = true)]
    public async Task<IActionResult> UpdateTask(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromBody] UpdateTaskDto request,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateTaskInProject(request, cancellationToken);
        return FromResult(result);
    }

    [HttpDelete("{groupId}/{projectId}/tasks/{taskId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.DeleteTask], CheckRoute = true)]
    public async Task<IActionResult> DeleteTask(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromRoute] int taskId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.DeleteTaskFromProject(new(groupId, projectId, taskId), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/{projectId}/tasks/mark/{taskId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.MarkTaskAsComplete], CheckRoute = true)]
    public async Task<IActionResult> MarkCompleteTask(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromRoute] int taskId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.MakeCompleteOrInCompleteTask(new(groupId, projectId, taskId), true, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{groupId}/{projectId}/tasks/unmark/{taskId}")]
    [GroupRolePermission(Permissions = [GroupRolePermissionConstants.MarkTaskAsInComplete], CheckRoute = true)]
    public async Task<IActionResult> MarkInCompleteTask(
        [FromRoute] int groupId,
        [FromRoute] int projectId,
        [FromRoute] int taskId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.MakeCompleteOrInCompleteTask(new(groupId, projectId, taskId), false, cancellationToken);
        return FromResult(result);
    }
}
