using Domain.Enums;

namespace Application.Features.UserTasks.Commands.UpdateUserTask;

public sealed record UpdateUserTaskPriorityCommand(
    int Id,
    TaskPriority Priority
) : IRequest<Result>;
