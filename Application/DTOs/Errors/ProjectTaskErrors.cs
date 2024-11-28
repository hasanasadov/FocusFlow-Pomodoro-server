namespace Application.DTOs.Errors;

public static class ProjectTaskErrors
{
    public static ErrorDesc TaskNotFound => new ErrorDesc("Task not found", "The task you are trying to update does not exist.");
}