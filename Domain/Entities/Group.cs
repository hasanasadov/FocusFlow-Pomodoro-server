using Domain.Events;

namespace Domain.Entities;

public sealed class Group : BaseEventEntity
{
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public string Photo { get; set; }
    public DateTime CreatedDate { get; private set; }

    public List<GroupMembership> Memberships { get; set; } = [];
    public List<Project> Projects { get; set; } = [];

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public static Group CreateGroup(Guid userId, string name, string description)
    {
        Project project = new()
        {
            Name = "Welcome",
            Description = "This is Welcome Project for new user.",
            StartDate = DateTime.UtcNow
        };
        Group group = new() { Name = name, Description = description, Projects = [project], UserId = userId, CreatedDate = DateTime.UtcNow };

        group.AddDomainEvent(new GroupCreatedDomainEvent(group, userId.ToString()));
        return group;
    }

    public void AddMember(Guid userId, int groupRoleId)
    {
        GroupMembership membership = GroupMembership.Create(userId, Id, groupRoleId);
        Memberships.Add(membership);
        AddDomainEvent(new MemberAddedDomainEvent(membership));
    }

    public bool RemoveMember(Guid userId)
    {
        GroupMembership membership = Memberships.FirstOrDefault(m => m.UserId == userId);
        if (membership is not null)
        {
            Memberships.Remove(membership);
            AddDomainEvent(new MemberRemovedDomainEvent(membership));
            return true;
        }
        return false;
    }

    public void AddProject(Project project)
    {
        Projects.Add(project);
    }

    public void RemoveProject(Project project)
    {
        Projects.Remove(project);
    }

    public void ChangeOwner(Guid userId)
    {
        UserId = userId;
    }
}