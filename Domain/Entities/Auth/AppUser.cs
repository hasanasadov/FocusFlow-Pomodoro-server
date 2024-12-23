using Domain.Entities.Tasks;

namespace Domain.Entities.Auth;

public class AppUser : IdentityUser<Guid>
{
    public string RefreshToken { get; set; } = "";
    public string PictureUrl { get; set; }
    public List<UserTask> UserTasks { get; set; } = [];
    public List<GroupMembership> GroupMemberships { get; set; } = [];
    public List<Project> Projects { get; set; } = [];
    public List<ProjectTask> AssignedProjectTasks { get; set; }


    public int GetTotalTasksCount() => UserTasks.Count;
    public int GetCompletedTasksCount() => UserTasks.Count(x => x.IsCompleted);
    public int GetUncompletedTasksCount() => GetTotalTasksCount() - GetCompletedTasksCount();

    public (List<UserTask> must, List<UserTask> should, List<UserTask> could, List<UserTask> wont) GetTasksByPriority()
    {
        var must = UserTasks.Where(x => x.Priority == TaskPriority.Must).ToList();
        var should = UserTasks.Where(x => x.Priority == TaskPriority.Should).ToList();
        var could = UserTasks.Where(x => x.Priority == TaskPriority.Could).ToList();
        var wont = UserTasks.Where(x => x.Priority == TaskPriority.Wont).ToList();

        return (must, should, could, wont);
    }
}