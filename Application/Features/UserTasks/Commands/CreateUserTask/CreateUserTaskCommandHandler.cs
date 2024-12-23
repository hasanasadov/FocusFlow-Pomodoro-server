﻿using Application.Services;

namespace Application.Features.UserTasks.Commands.CreateUserTask;

public class CreateUserTaskCommandHandler(IUserTaskService UserTaskService) : IRequestHandler<CreateUserTaskCommand, Result>
{
    private readonly IUserTaskService userTaskService = UserTaskService;

    public async Task<Result> Handle(CreateUserTaskCommand request, CancellationToken cancellationToken)
    {
        return await userTaskService.CreateTask(new(request.Title, request.Description, request.Label, request.DueDate, request.Priority, request.Status, request.IsCompleted), cancellationToken);
    }
}
