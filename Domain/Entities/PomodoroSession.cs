using Domain.Entities.Tasks;

namespace Domain.Entities;

public sealed class PomodoroSession : BaseEntity
{
    public Pomodoro Pomodoro { get; set; }
    public int PomodoroId { get; set; }

    public DateTime StartedTime { get; set; }
    public DateTime FinishedTime { get; set; }

    public List<TaskEntity> CompletedTasks { get; set; }
    public TaskEntity RecentTask { get; set; }
}

public sealed class Pomodoro : BaseEntity
{
    public AppUser User { get; set; }
    public Guid UserId { get; set; }
    public int SessionCount { get; set; }
    public TimeOnly PomodoroTime { get; set; }
    public List<PomodoroSession> Sessions { get; set; }
}
