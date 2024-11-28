using Application.Services;

namespace Application.Features.UserTasks.Queries.GetAllUserTasksByLabel;

public sealed class GetAllUserTasksByLabelQueryHandler(IUserTaskService UserTaskService) :         
                            IRequestHandler<GetAllUserTasksByLabelQuery, Result<IReadOnlyCollection<UserTaskDto>>>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result<IReadOnlyCollection<UserTaskDto>>> Handle(GetAllUserTasksByLabelQuery request, CancellationToken cancellationToken)
    {
        return await userTaskService.GetAllUserTaskByLabel(request.Label, cancellationToken);
    }
}