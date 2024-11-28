using Domain.Entities.Tasks;
using Infrastructure.Persistence.Context;
using Infrastructure.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services;

public sealed class UserTaskService(AppDbContext Context, IUserContext UserContext, IHubContext<SignalHub> HubContext) : IUserTaskService
{
    private readonly AppDbContext _dbContext = Context;
    private readonly DbSet<UserTask> _userTasks = Context.UserTasks;
    private readonly IUserContext _userContext = UserContext;
    private readonly IHubContext<SignalHub> _hubContext = HubContext;

    public async Task<Result<UserTaskDto>> GetTaskById(int id, CancellationToken cancellationToken)
    {
        UserTask? userTask = await _userTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (userTask == null)
        {
            return Result<UserTaskDto>.Failure(Error.NotFound(UserTaskErrors.UserTaskNotFound));
        }
        if (userTask.AppUserId != _userContext.UserId)
        {
            return Result<UserTaskDto>.Failure(Error.InvalidRequest(UserTaskErrors.UnauthorizedAccess));
        }
        return Result<UserTaskDto>.Success(MapToUserTaskDto(userTask, true));
    }

    public async Task<Result<IReadOnlyCollection<UserTaskDto>>> GetTasksByUserId(CancellationToken cancellationToken)
    {
        var userTasks = await _userTasks.Include(x => x.Steps)
                                        .AsNoTracking()
                                        .Where(x => x.AppUserId == _userContext.UserId)
                                        .ToListAsync(cancellationToken);

        var userTaskDtos = userTasks.Select(userTask => MapToUserTaskDto(userTask, true)).ToList();
        return Result<IReadOnlyCollection<UserTaskDto>>.Success(userTaskDtos);
    }


    public async Task<Result<UserTaskPriorityDto>> GetTasksByUserIdGroupByPriorityList(CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<UserTaskDto>> userTasksResult = await GetTasksByUserId(cancellationToken);
        if (userTasksResult.IsFailure)
        {
            return Result<UserTaskPriorityDto>.Failure(userTasksResult.Error);
        }
        IReadOnlyCollection<UserTaskDto> userTasks = userTasksResult.Value;
        
        var tasks = new List<UserTaskListDto>
        {
            new(0,  "Must",   "Red",   []),
            new(1,  "Should", "Blue",  []),
            new(2,  "Could",  "Green", []),
            new(3,  "Won't",  "Grey",  [])
        };
        int total = userTasks.Count, completed = 0;

        foreach (var userTask in userTasks)
        {
            if (userTask.IsCompleted)
            {
                completed++;
            }

            switch (userTask.Priority)
            {
                case TaskPriority.Must:
                    tasks[0].Items.Add(userTask);
                    break;
                case TaskPriority.Should:
                    tasks[1].Items.Add(userTask);
                    break;
                case TaskPriority.Could:
                    tasks[2].Items.Add(userTask);
                    break;
                case TaskPriority.Wont:
                    tasks[3].Items.Add(userTask);
                    break;
            }
        }
        return Result<UserTaskPriorityDto>.Success(new UserTaskPriorityDto(total, total - completed, completed, tasks));
    }

    public async Task<Result<IReadOnlyCollection<string>>> GetAllLabelsOfUser(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<string> labels = await _userTasks.AsNoTracking()
                                                .Where(x => x.AppUserId == _userContext.UserId)
                                                .Select(x => x.Label)
                                                .Distinct()
                                                .ToListAsync();

        return Result<IReadOnlyCollection<string>>.Success(labels);
    }

    public async Task<Result<IReadOnlyCollection<UserTaskDto>>> GetAllUserTaskByLabel(string label, CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<UserTaskDto>> userTasksResult = await GetTasksByUserId(cancellationToken);
        if (userTasksResult.IsFailure)
        {
            return Result<IReadOnlyCollection<UserTaskDto>>.Failure(userTasksResult.Error);
        }
        IReadOnlyCollection<UserTaskDto> userTasks = userTasksResult.Value;
        
        return Result<IReadOnlyCollection<UserTaskDto>>.Success(userTasks.Where(x => x.Label.Equals(label, StringComparison.InvariantCultureIgnoreCase)).ToList());
    }

    public async Task<Result<IReadOnlyCollection<A>>> Get28DayActivity(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<UserTaskDto> userTasksResult = (await GetTasksByUserId(cancellationToken)).Value;

        var dates = userTasksResult.Where(x => x.StartDate >= DateTime.UtcNow - TimeSpan.FromDays(28)
                                               && x.IsCompleted)
                                   .DistinctBy(x=>x.StartDate.Day) 
                                   .Select(x => new A( x.StartDate.Day, x.IsCompleted ))
                                   .ToList();
        
        return Result<IReadOnlyCollection<A>>.Success(dates);
    }

    public async Task<Result> CreateTask(CreateUserTaskDto task, CancellationToken cancellationToken)
    {
        var userTask = new UserTask
        {
            AppUserId = _userContext.UserId,
            Title = task.Title,
            Description = task.Description,
            Label = task.Label.ToLower(),
            DueDate = task.DueDate,
            Priority = task.Priority,
            Status = task.Status,
            IsCompleted = task.IsCompleted
        };

        await _userTasks.AddAsync(userTask, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);


        var userTaskDto = MapToUserTaskDto(userTask);

        

        Console.WriteLine($"Sending CreatedNewUserTask to user {_userContext.UserId}");
        await _hubContext.Clients.All
            .SendAsync("CreatedNewUserTask", userTaskDto, cancellationToken);
        Console.WriteLine("CreatedNewUserTask sent");

        return Result.Success();
    }

    public async Task<Result> CreateListTasks(List<CreateUserTaskListDto> tasks, CancellationToken cancellationToken)
    {
        foreach (var task in tasks)
        {
            var userTask = new UserTask
            {
                AppUserId = _userContext.UserId,
                Title = task.Title,
                Description = task.Description,
                Label = task.Label.ToLower(),
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                IsCompleted = task.IsCompleted,
                Steps  = task.StepDtos.Select(step => new TaskStep
                {
                    Description = step.Description,
                    IsCompleted = step.IsCompleted
                }).ToHashSet()
            };

            await _userTasks.AddAsync(userTask, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);


        return Result.Success();
    }

    public async Task<Result> UpdateTask(UpdateUserTaskDto task, CancellationToken cancellationToken)
    {
        var userTask = await _userTasks.FirstOrDefaultAsync(x => x.Id == task.Id, cancellationToken);
        if (userTask == null)
        {
            return Result.Failure(Error.NotFound(UserTaskErrors.UserTaskNotFound));
        }
        if (userTask.AppUserId != _userContext.UserId)
        {
            return Result.Failure(Error.InvalidRequest(UserTaskErrors.UnauthorizedAccess));
        }

        userTask.Title = task.Title;
        userTask.Description = task.Description;
        userTask.Label = task.Label.ToLower();
        userTask.DueDate = task.DueDate;
        userTask.Priority = task.Priority;
        userTask.IsCompleted = task.IsCompleted;
        userTask.Status = task.Status;

        _userTasks.Update(userTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var userTaskDto = MapToUserTaskDto(userTask);
        await _hubContext.Clients.User(_userContext.UserId.ToString())
            .SendAsync("UpdatedNewUserTask", _userContext.UserId.ToString(), userTaskDto, cancellationToken);


        return Result.Success();
    }

    public async Task<Result> UpdateTaskPriority(UpdateUserTaskPriorityDto priorityDto, CancellationToken cancellationToken)
    {
        var userTask = await _userTasks.FirstOrDefaultAsync(x => x.AppUserId == _userContext.UserId && x.Id == priorityDto.Id, cancellationToken);
        
        if (userTask == null)
        {
            return Result.Failure(Error.NotFound(UserTaskErrors.UserTaskNotFound));
        }
        userTask.Priority = priorityDto.Priority;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var userTaskDto = MapToUserTaskDto(userTask);
        await _hubContext.Clients.User(_userContext.UserId.ToString())
            .SendAsync("UpdateUserTaskPriority", _userContext.UserId.ToString(), userTaskDto, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CompleteTask(int id, CancellationToken cancellationToken)
    {
        var userTask = await _userTasks.FirstOrDefaultAsync(x => x.Id == id && x.AppUserId.Equals(_userContext.UserId), cancellationToken);
        if (userTask == null)
        {
            return Result.Failure(Error.NotFound(UserTaskErrors.UserTaskNotFound));
        }

        userTask.MarkAsCompleted();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var userTaskDto = MapToUserTaskDto(userTask);
        await _hubContext.Clients.User(_userContext.UserId.ToString())
            .SendAsync("CompletedUserTask", _userContext.UserId.ToString(), userTaskDto, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteTask(int id, CancellationToken cancellationToken)
    {
        var userTask = await _userTasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (userTask == null)
        {
            return Result.Failure(Error.NotFound(UserTaskErrors.UserTaskNotFound));
        }
        if (userTask.AppUserId != _userContext.UserId)
        {
            return Result.Failure(Error.InvalidRequest(UserTaskErrors.UnauthorizedAccess));
        }

        _userTasks.Remove(userTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var userTaskDto = MapToUserTaskDto(userTask);
        await _hubContext.Clients.User(_userContext.UserId.ToString())
            .SendAsync("DeletedUserTask", _userContext.UserId.ToString(), userTaskDto, cancellationToken);


        return Result.Success();
    }


    #region Helper
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UserTaskDto MapToUserTaskDto(UserTask userTask, bool isGet = false)
    {
        return new UserTaskDto(
            userTask.Id,
            userTask.Title,
            userTask.Description,
            userTask.Label,
            userTask.StartDate,
            userTask.DueDate,
            userTask.FinishDate,
            userTask.Priority,
            userTask.Status,
            userTask.IsCompleted,
            isGet ? userTask.Steps.Select(step =>
            new TaskStepDto(step.Id,
                            step.Description,
                            step.IsCompleted)).ToList():
            []
        );
    }
    #endregion
}
