using Application.Services;

namespace Application.Features.UserTasks.Commands.CompleteTask;

public sealed record CompleteTaskCommand(int id) : IRequest<Result>;


public sealed class CompleteTaskCommandHandler(IUserTaskService UserTaskService) : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        return await userTaskService.CompleteTask(request.id, cancellationToken);
    }
}