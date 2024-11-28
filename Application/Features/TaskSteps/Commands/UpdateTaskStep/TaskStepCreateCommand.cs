

namespace Application.Features.TaskSteps.Commands.UpdateTaskStep;
public sealed record UpdateTaskStepCommand(int Id, string Description, int UserTaskId, bool IsCompleted = false) : IRequest<Result>;
