namespace Application.Features.UserTasks.Queries.GetAllUserTasksByLabel;
public sealed record GetAllUserTasksByLabelQuery(string Label) : IRequest<Result<IReadOnlyCollection<UserTaskDto>>>;
