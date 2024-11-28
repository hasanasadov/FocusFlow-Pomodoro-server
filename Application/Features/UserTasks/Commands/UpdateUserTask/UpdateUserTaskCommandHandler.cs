using Application.Services;

namespace Application.Features.UserTasks.Commands.UpdateUserTask;

public sealed class UpdateUserTaskCommandHandler : IRequestHandler<UpdateUserTaskCommand, Result>
{
    private readonly IUserTaskService _userTaskService;

    public UpdateUserTaskCommandHandler(IUserTaskService userTaskService)
    {
        _userTaskService = userTaskService;
    }

    public async Task<Result> Handle(UpdateUserTaskCommand request, CancellationToken cancellationToken)
    {
        return await _userTaskService.UpdateTask(new(request.Id, request.Title, request.Description, request.Label, request.DueDate, request.Priority, request.Status, request.IsCompleted), cancellationToken);
    }
}