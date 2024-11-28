namespace Application.Features.TaskSteps.Commands.CreateTaskStep;


public sealed record CreateTaskStepCommand(string Description, int UserTaskId, bool IsCompleted = false) : IRequest<Result>;
