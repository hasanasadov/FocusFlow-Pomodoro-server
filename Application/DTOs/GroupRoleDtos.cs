namespace Application.DTOs;

public sealed record UpdateGroupRoleDto(int GroupRoleId, string Name, List<string> Permissions);
public sealed record CreateGroupRoleDto(string Name, List<string> Permissions);
public sealed record GroupRoleDto(int Id, string Name, List<string> Permissions);