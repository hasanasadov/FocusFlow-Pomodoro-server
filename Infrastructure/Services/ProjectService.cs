using Domain.Entities.Tasks;

namespace Infrastructure.Services;

public sealed class ProjectService(
    AppDbContext dbContext,
    IUserContext userContext) : IProjectService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result<ProjectDto>> GetProjectAsync(ProjectRequest request, CancellationToken cancellationToken)
    {
        Project? project = await _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Users)
            .FirstOrDefaultAsync(project => project.Id == request.ProjectId && 
                                 project.GroupId == request.GroupId, cancellationToken);

        if (project == null)
        {
            return Result<ProjectDto>.Failure(Error.NotFound(ProjectErrors.ProjectNotFound));
        }
        return Result<ProjectDto>.Success(new ProjectDto(project.Id, project.Name, project.Description,
                project.Users
                .Select(user => new AppUserDto(nameof(user.Id), user.UserName!, user.Email!, user.PictureUrl))
                .ToList())
            );
    }

    public async Task<Result<List<ProjectDto>>> GetAllProjectsAsync(int groupId, CancellationToken cancellationToken)
    {
        //TODO: Pagination
        Group? group = await _dbContext
            .Groups
            .AsNoTracking()
            .Include(x => x.Projects)
            .ThenInclude(x => x.Users)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);

        if (group == null)
        {
            return Result<List<ProjectDto>>.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }

        return Result<List<ProjectDto>>.Success(group.Projects
            .Select(project => new ProjectDto(project.Id, project.Name, project.Description,
                project.Users
                .Select(user => new AppUserDto(nameof(user.Id), user.UserName!, user.Email!, user.PictureUrl))
                .ToList()))
            .ToList());
    }

    public async Task<Result> AddOrRemoveUserToProject(string usernameOrEmail, ProjectRequest request, bool isRemove, CancellationToken cancellationToken)
    {
        Project? project = await _dbContext.Projects
            .Include(project => project.Users)
            .FirstOrDefaultAsync(project => project.Id == request.ProjectId &&
                                            project.GroupId == request.GroupId, cancellationToken);

        if (project is null)
        {
            return Result.Failure(Error.NotFound(ProjectErrors.ProjectNotFound));
        }

        AppUser? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserName == usernameOrEmail || user.Email == usernameOrEmail, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound(AuthErrors.UserNotFound));
        }
        if (isRemove)
        {
            project.RemoveUser(user);
        }
        else
        {
            project.AddUser(user);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    public async Task<Result> CreateProjectAsync(int groupId, CreateProjectDto project, CancellationToken cancellationToken)
    {
        Group? group = await _dbContext.Groups
            .FirstOrDefaultAsync(group => group.Id == groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(Error.NotFound(GroupErrors.GroupNotFound));
        }
        group.AddProject(Project.CreateProject(project.Name, project.Description, DateTime.UtcNow, project.dueDate));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateProjectAsync(int groupId, UpdateProjectDto project, CancellationToken cancellationToken)
    {
        Project? dbProject = await _dbContext.Projects
            .FirstOrDefaultAsync(project => project.Id == project.Id && project.GroupId == groupId, cancellationToken);

        if (dbProject is null)
        {
            return Result.Failure(Error.NotFound(ProjectErrors.ProjectNotFound));
        }
        dbProject.DueDate = project.DueDate;
        dbProject.Description = project.Description;
        dbProject.Name = project.Name;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteProjectAsync(ProjectRequest request, CancellationToken cancellationToken)
    {
        bool dbProject = await _dbContext.Projects
            .AnyAsync(project => project.Id == request.ProjectId && project.GroupId == request.GroupId, cancellationToken);

        if (!dbProject)
        {
            return Result.Failure(Error.NotFound(ProjectErrors.ProjectNotFound));
        }

        await _dbContext.Projects
            .Where(project => project.Id == request.ProjectId)
            .ExecuteDeleteAsync();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AddTaskToProject(ProjectRequest request, CreateTaskDto task, CancellationToken cancellationToken)
    {
        Project? project = await _dbContext.Projects
            .Include(project => project.Tasks)
            .FirstOrDefaultAsync(project => project.Id == request.ProjectId && project.GroupId == request.GroupId, cancellationToken);

        if (project is null)
        {
            return Result.Failure(Error.NotFound(ProjectErrors.ProjectNotFound));
        }

        HashSet<string> usersSet = new HashSet<string>(task.usernamesOrEmails.Select(x => x.ToUpper()));

        List<AppUser> assignedUsers = await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                usersSet.Contains(user.NormalizedEmail!) ||
                usersSet.Contains(user.NormalizedUserName!))
            .ToListAsync(cancellationToken);

        project.AddTask(ProjectTask.Create(task.Title, task.Description, task.Label, task.DueDate, task.Priority, assignedUsers));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateTaskInProject(UpdateTaskDto task, CancellationToken cancellationToken)
    {
        ProjectTask? projectTask = await _dbContext.ProjectTasks
            .Include(projectTask => projectTask.AssignedUsers)
            .FirstOrDefaultAsync(projectTask => projectTask.Id == task.TaskId, cancellationToken);

        if (projectTask is null)
        {
            return Result.Failure(Error.NotFound(ProjectTaskErrors.TaskNotFound));
        }

        HashSet<string> usersSet = new HashSet<string>(task.usernamesOrEmails.Select(x => x.ToUpper()));

        List<AppUser> assignedUsers = await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                usersSet.Contains(user.NormalizedEmail!) ||
                usersSet.Contains(user.NormalizedUserName!))
            .ToListAsync(cancellationToken);

        projectTask.Update(projectTask.Title, task.Description, task.Label, task.DueDate, task.Priority, assignedUsers);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteTaskFromProject(ProjectTaskRequest request, CancellationToken cancellationToken)
    {
        ProjectTask? projectTask = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(projectTask => projectTask.Id == request.TaskId
            && projectTask.ProjectId == request.ProjectId
            && projectTask.Project.GroupId == request.GroupId, cancellationToken);

        if (projectTask is null)
        {
            return Result.Failure(Error.NotFound(ProjectTaskErrors.TaskNotFound));
        }

        _dbContext.ProjectTasks.Remove(projectTask);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> MakeCompleteOrInCompleteTask(ProjectTaskRequest request, bool isComplete, CancellationToken cancellationToken)
    {
        ProjectTask? projectTask = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(projectTask => projectTask.Id == request.TaskId
            && projectTask.ProjectId == request.ProjectId
            && projectTask.Project.GroupId == request.GroupId,
                                 cancellationToken);

        if (projectTask is null)
        {
            return Result.Failure(Error.NotFound(ProjectTaskErrors.TaskNotFound));
        }
        if (isComplete)
        {
            projectTask.MarkAsCompleted();
        }
        else
        {
            projectTask.MarkAsInCompleted();
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AddOrRemoveUserToTask(List<string> usernameOrEmails, ProjectTaskRequest request, bool isRemove, CancellationToken cancellationToken)
    {
        ProjectTask? projectTask = await _dbContext.ProjectTasks
            .Include(projectTask => projectTask.AssignedUsers)
            .FirstOrDefaultAsync(projectTask => projectTask.Id == request.TaskId
            && projectTask.ProjectId == request.ProjectId
            && projectTask.Project.GroupId == request.GroupId, cancellationToken);

        if (projectTask is null)
        {
            return Result.Failure(Error.NotFound(ProjectTaskErrors.TaskNotFound));
        }

        HashSet<string> usersSet = new HashSet<string>(usernameOrEmails.Select(x => x.ToUpper()));

        List<AppUser> assignedUsers = await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                usersSet.Contains(user.NormalizedEmail!) ||
                usersSet.Contains(user.NormalizedUserName!))
            .ToListAsync(cancellationToken);
        if (isRemove)
        {
            projectTask.RemoveUsers(assignedUsers);
        }
        else
        {
            projectTask.AssignUsers(assignedUsers);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
