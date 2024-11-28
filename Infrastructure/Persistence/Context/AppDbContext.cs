using Domain.Entities.Common;
using Domain.Entities.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserTask> UserTasks { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<TaskStep> TaskSteps { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    public DbSet<GroupMembership> GroupMemberships { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<GroupRole>().HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<TaskEntity>().UseTpcMappingStrategy();

        builder.Entity<ProjectTask>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<UserTask>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync();
        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var _publisher = this.GetService<IMediator>();
        var domainEvents = ChangeTracker
            .Entries<BaseEventEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}

