using Domain.Enums;

namespace Application.DTOs;

public sealed record CreateUserTaskDto(string Title, string Description, string Label, DateTime DueDate, TaskPriority Priority, UserTaskStatus Status, bool IsCompleted);

public sealed record CreateUserTaskListDto(string Title, string Description, string Label, DateTime DueDate, TaskPriority Priority, UserTaskStatus Status, bool IsCompleted, List<CreateTaskStepDtoForList> StepDtos);

public sealed record CreateTaskStepDtoForList(string Description, bool IsCompleted);

public sealed record UpdateUserTaskDto(int Id, string Title, string Description, string Label, DateTime DueDate, TaskPriority Priority, UserTaskStatus Status, bool IsCompleted);

public sealed record UpdateUserTaskPriorityDto(int Id, TaskPriority Priority);

public sealed record UserTaskDto(int Id, string Title, string Description, string Label, DateTime StartDate, DateTime DueDate, DateTime FinishedDate, TaskPriority Priority, UserTaskStatus Status, bool IsCompleted, List<TaskStepDto> StepDtos);

public sealed record UserTaskListDto(int Id, string Title, string Color, List<UserTaskDto> Items);

public sealed record UserTaskPriorityDto(int Total, int Pending, int Completed, List<UserTaskListDto> Tasks);
