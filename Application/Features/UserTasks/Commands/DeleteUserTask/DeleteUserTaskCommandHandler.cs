using Application.Services;

namespace Application.Features.UserTasks.Commands.DeleteUserTask;

public sealed class DeleteUserTaskCommandHandler(IUserTaskService UserTaskService) : IRequestHandler<DeleteUserTaskCommand, Result>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result> Handle(DeleteUserTaskCommand request, CancellationToken cancellationToken)
    {
        return await userTaskService.DeleteTask(request.Id, cancellationToken);
    }
}
