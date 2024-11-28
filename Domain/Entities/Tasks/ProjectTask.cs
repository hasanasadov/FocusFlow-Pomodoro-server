namespace Domain.Entities.Tasks;

public sealed class ProjectTask : TaskEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public HashSet<AppUser> AssignedUsers { get; set; } = [];

    public static ProjectTask Create(string title,
                              string description,
                              string label,
                              DateTime dueDate,
                              TaskPriority priority,
                              List<AppUser> assignedUsers)
    {
        {
            var projectTask = new ProjectTask
            {
                Title = title,
                Description = description,
                Label = label,
                DueDate = dueDate,
                Priority = priority,
                AssignedUsers = assignedUsers.ToHashSet()
            };

            return projectTask;
        }
    }

    public void Update(string title,
                       string description,
                       string label,
                       DateTime dueDate,
                       TaskPriority priority,
                       List<AppUser> assignedUsers)
    {
        Title = title;
        Description = description;
        Label = label;
        DueDate = dueDate;
        Priority = priority;

        HashSet<Guid> newUserIds = assignedUsers.Select(x => x.Id).ToHashSet();
        HashSet<Guid> currentIds = AssignedUsers.Select(x => x.Id).ToHashSet();

        List<AppUser> addedUsers = assignedUsers.Where(x => !currentIds.Contains(x.Id)).ToList();
        List<AppUser> removedUsers = AssignedUsers.Where(x => !newUserIds.Contains(x.Id)).ToList();

        AssignUsers(addedUsers);
        RemoveUsers(removedUsers);
    }

    public void AssignUsers(List<AppUser> user)
    {
        foreach (var appUser in user)
        {
            if (!AssignedUsers.Contains(appUser))
            {
                AssignedUsers.Add(appUser);
            }
        }
    }

    public void RemoveUsers(List<AppUser> user)
    {
        foreach (var appUser in user)
        {
            if (AssignedUsers.Contains(appUser))
            {
                AssignedUsers.Remove(appUser);
            }
        }
    }
}