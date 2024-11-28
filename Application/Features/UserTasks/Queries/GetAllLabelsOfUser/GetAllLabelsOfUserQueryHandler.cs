using Application.Services;

namespace Application.Features.UserTasks.Queries.GetAllLabelsOfUser;

public sealed class GetAllLabelsOfUserQueryHandler(IUserTaskService UserTaskService) : IRequestHandler<GetAllLabelsOfUserQuery, Result<IReadOnlyCollection<string>>>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result<IReadOnlyCollection<string>>> Handle(GetAllLabelsOfUserQuery request, CancellationToken cancellationToken)
    {
        return await userTaskService.GetAllLabelsOfUser(cancellationToken);
    }
}