using Application.Services;

namespace Application.Features.TaskSteps.Commands.UpdateTaskStep;

public sealed class UpdateTaskStepCommandHandler(ITaskStepService TaskStepService) : IRequestHandler<UpdateTaskStepCommand, Result>
{
    private readonly ITaskStepService taskStepService = TaskStepService;

    public async Task<Result> Handle(UpdateTaskStepCommand request, CancellationToken cancellationToken)
    {
        return await taskStepService.UpdateTaskStep(new(request.Id, request.Description, request.IsCompleted), cancellationToken);
    }
}
