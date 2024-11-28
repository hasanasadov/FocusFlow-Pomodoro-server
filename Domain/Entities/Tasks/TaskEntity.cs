namespace Domain.Entities.Tasks;

public abstract class TaskEntity : BaseEntity
{
    public string Title { get; set; } = "New Task.";
    public string Description { get; set; } = "New Task Description.";
    public string Label { get; set; } = "new";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime FinishDate { get; private set; } = default;
    public bool IsCompleted { get; set; } = false;
    public TaskPriority Priority { get; set; } = TaskPriority.Wont;
    public UserTaskStatus Status { get; set; } = UserTaskStatus.Todo;

    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;
        FinishDate = DateTime.UtcNow;
    }

    public void MarkAsInCompleted()
    {
        if (!IsCompleted)
        {
            return;
        }

        IsCompleted = false;
        FinishDate = default;
    }
}