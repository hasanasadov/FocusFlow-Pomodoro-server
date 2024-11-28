using Application.Services;

namespace Application.Features.TaskSteps.Commands.CreateTaskStep;

public sealed class CreateTaskStepCommandHandler(ITaskStepService TaskStepService) : IRequestHandler<CreateTaskStepCommand, Result>
{
    private readonly ITaskStepService taskStepService = TaskStepService;

    public async Task<Result> Handle(CreateTaskStepCommand request, CancellationToken cancellationToken)
    {
        return await taskStepService.CreateTaskStep(new(request.Description, request.UserTaskId, request.IsCompleted), cancellationToken);
    }
}
