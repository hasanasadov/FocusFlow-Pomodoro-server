using Application.Services;

namespace Application.Features.UserTasks.Commands.UpdateUserTask;

public sealed class UpdateUserTaskPriorityCommandHandler : IRequestHandler<UpdateUserTaskPriorityCommand, Result>
{
    private readonly IUserTaskService _userTaskService;

    public UpdateUserTaskPriorityCommandHandler(IUserTaskService userTaskService)
    {
        _userTaskService = userTaskService;
    }

    public async Task<Result> Handle(UpdateUserTaskPriorityCommand request, CancellationToken cancellationToken)
    {
        return await _userTaskService.UpdateTaskPriority(new(request.Id, request.Priority), cancellationToken);
    }
}