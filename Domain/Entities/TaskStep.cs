using Domain.Entities.Tasks;

namespace Domain.Entities;

public sealed class TaskStep : BaseEntity
{
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int UserTaskId { get; set; }
    public UserTask UserTask { get; set; }
}
