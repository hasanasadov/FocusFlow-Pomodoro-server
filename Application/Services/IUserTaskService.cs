namespace Application.Services;

public interface IUserTaskService
{
    Task<Result> CreateTask(CreateUserTaskDto task, CancellationToken cancellationToken);
    Task<Result> CreateListTasks(List<CreateUserTaskListDto> tasks, CancellationToken cancellationToken);

    
    Task<Result> UpdateTask(UpdateUserTaskDto task, CancellationToken cancellationToken);
    Task<Result> UpdateTaskPriority(UpdateUserTaskPriorityDto priorityDto, CancellationToken cancellationToken);
    Task<Result> CompleteTask(int id, CancellationToken cancellationToken);
    Task<Result> DeleteTask(int id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<A>>> Get28DayActivity(CancellationToken cancellationToken);
    Task<Result<UserTaskDto>> GetTaskById(int id, CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<UserTaskDto>>> GetTasksByUserId(CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<UserTaskDto>>> GetAllUserTaskByLabel(string label, CancellationToken cancellationToken);

    Task<Result<UserTaskPriorityDto>> GetTasksByUserIdGroupByPriorityList(CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<string>>> GetAllLabelsOfUser(CancellationToken cancellationToken);
    
}

public sealed record A(int Day, bool IsCompleted);