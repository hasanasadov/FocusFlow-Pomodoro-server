namespace Domain.Entities.Tasks;

public sealed class UserTask : TaskEntity
{
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    public HashSet<TaskStep> Steps { get; set; }
}