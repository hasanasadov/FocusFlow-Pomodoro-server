namespace Application.DTOs.Errors;

public static class ProjectErrors
{
    public static ErrorDesc ProjectNotFound => new("Project not found", "The project was not found.");
    public static ErrorDesc UserAlreadyInProject => new("User already in project", "The user is already a member of the project.");
    public static ErrorDesc UserNotInProject => new("User not in project", "The user is not a member of the project.");
    public static ErrorDesc ProjectAlreadyExists => new("Project already exists", "A project with the same name already exists.");
    
}