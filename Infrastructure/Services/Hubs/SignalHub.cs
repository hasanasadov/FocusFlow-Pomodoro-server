using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services.Hubs;
public sealed class SignalHub : Hub
{
    public async Task CreatedNewUserTask(string userId, UserTaskDto userTaskDto, CancellationToken cancellationToken)
    {
        await Clients.User(userId).SendAsync("CreatedNewUserTask", userTaskDto, cancellationToken);
    }

    public async Task UpdatedNewUserTask(string userId, UserTaskDto userTaskDto, CancellationToken cancellationToken)
    {
        await Clients.User(userId).SendAsync("UpdatedNewUserTask", userTaskDto, cancellationToken);
    }

    public async Task UpdateUserTaskPriority(string userId, UserTaskDto userTaskDto, CancellationToken cancellationToken)
    {
        await Clients.User(userId).SendAsync("UpdateUserTaskPriority", userTaskDto, cancellationToken);
    }

    public async Task DeletedUserTask(string userId, UserTaskDto userTaskDto, CancellationToken cancellationToken)
    {
        await Clients.User(userId).SendAsync("DeletedUserTask", userTaskDto, cancellationToken);
    }
}
