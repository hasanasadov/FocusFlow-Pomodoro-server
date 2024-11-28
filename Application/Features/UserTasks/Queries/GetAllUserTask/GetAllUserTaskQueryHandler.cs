using Application.Services;

namespace Application.Features.UserTasks.Queries.GetAllUserTask;

public sealed class GetAllUserTaskQueryHandler(IUserTaskService UserTaskService) : IRequestHandler<GetAllUserTaskQuery, Result<IReadOnlyCollection<UserTaskDto>>>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result<IReadOnlyCollection<UserTaskDto>>> Handle(GetAllUserTaskQuery request, CancellationToken cancellationToken)
    {
        return await userTaskService.GetTasksByUserId(cancellationToken);
    }
}