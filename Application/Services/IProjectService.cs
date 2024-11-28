namespace Application.Services;

public interface IProjectService
{
    Task<Result<ProjectDto>> GetProjectAsync(ProjectRequest request, CancellationToken cancellationToken);
    Task<Result<List<ProjectDto>>> GetAllProjectsAsync(int groupId, CancellationToken cancellationToken);

    Task<Result> CreateProjectAsync(int groupId, CreateProjectDto project, CancellationToken cancellationToken);
    Task<Result> UpdateProjectAsync(int groupId, UpdateProjectDto project, CancellationToken cancellationToken);
    Task<Result> DeleteProjectAsync(ProjectRequest request, CancellationToken cancellationToken);

    Task<Result> AddOrRemoveUserToProject(string usernameOrEmail, ProjectRequest request, bool isRemove, CancellationToken cancellationToken);

    Task<Result> AddTaskToProject(ProjectRequest request, CreateTaskDto task, CancellationToken cancellationToken);
    Task<Result> UpdateTaskInProject(UpdateTaskDto task, CancellationToken cancellationToken);
    Task<Result> DeleteTaskFromProject(ProjectTaskRequest request, CancellationToken cancellationToken);

    Task<Result> MakeCompleteOrInCompleteTask(ProjectTaskRequest request, bool isComplete, CancellationToken cancellationToken);
    Task<Result> AddOrRemoveUserToTask(List<string> usernameOrEmails, ProjectTaskRequest request, bool isRemove, CancellationToken cancellationToken);

}

public sealed record ProjectRequest(int GroupId, int ProjectId);
public sealed record ProjectTaskRequest(int GroupId, int ProjectId, int TaskId);