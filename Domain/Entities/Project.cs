using Domain.Entities.Tasks;

namespace Domain.Entities;


public sealed class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<AppUser> Users { get; set; }
    public List<ProjectTask> Tasks { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }

    public static Project CreateProject(string name, string description, DateTime startDate, DateTime? dueDate)
    {
        return new()
        {
            Name = name,
            Description = description,
            StartDate = startDate,
            DueDate = dueDate
        };
    }

    public void AddTask(ProjectTask task)
    {
        Tasks.Add(task);
    }

    public void AddUser(AppUser user)
    {
        if (!Users.Any(u => u.Id == user.Id))
            Users.Add(user);
    }

    public void RemoveUser(AppUser user)
    {
        if (Users.Any(u => u.Id == user.Id))
            Users.Remove(user);
    }
}