namespace Application.Features.UserTasks.Commands.DeleteUserTask;

public sealed record DeleteUserTaskCommand(int Id) : IRequest<Result>;