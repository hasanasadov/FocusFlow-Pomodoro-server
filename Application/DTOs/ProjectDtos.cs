using Domain.Enums;

namespace Application.DTOs;

public record ProjectDto(int Id, string Name, string Description, List<AppUserDto> Users);
public record UpdateProjectDto(int Id, string Name, string Description, DateTime? DueDate);
public record CreateProjectDto(string Name, string Description, DateTime? dueDate);

public sealed record CreateStepDto(string Name, string Description, string Label);

public sealed record CreateTaskDto(string Title,
                                   string Description,
                                   string Label,
                                   DateTime DueDate,
                                   TaskPriority Priority,
                                   List<string> usernamesOrEmails);
public sealed record UpdateTaskDto(int TaskId,
                                   string Title,
                                   string Description,
                                   string Label,
                                   DateTime DueDate,
                                   TaskPriority Priority,
                                   UserTaskStatus Status,
                                   List<string> usernamesOrEmails);