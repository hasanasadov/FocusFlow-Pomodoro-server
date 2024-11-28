namespace Application.Features.UserTasks.Queries.GetAllLabelsOfUser;

public sealed record GetAllLabelsOfUserQuery() : IRequest<Result<IReadOnlyCollection<string>>>;
