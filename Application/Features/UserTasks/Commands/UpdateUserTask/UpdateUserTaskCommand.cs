using Domain.Enums;

namespace Application.Features.UserTasks.Commands.UpdateUserTask;

public sealed record UpdateUserTaskCommand(
    int Id,
    string Title,
    string Description,
    string Label,
    DateTime DueDate,
    TaskPriority Priority,
    UserTaskStatus Status,
    bool IsCompleted = false
) : IRequest<Result>;
