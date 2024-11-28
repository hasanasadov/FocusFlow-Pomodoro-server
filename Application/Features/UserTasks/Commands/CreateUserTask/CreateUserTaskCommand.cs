using Domain.Enums;

namespace Application.Features.UserTasks.Commands.CreateUserTask;

// todo convert to record
public sealed record CreateUserTaskCommand(
    string Title,
    string Description,
    string Label,
    DateTime DueDate,
    TaskPriority Priority,
    UserTaskStatus Status,
    bool IsCompleted = false
) : IRequest<Result>;
