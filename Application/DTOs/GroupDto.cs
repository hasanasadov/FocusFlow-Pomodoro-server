using System.Text.Json.Serialization;

namespace Application.DTOs;

public sealed record GroupDto(int Id,
                              string Name,
                              string Description,
                              string PhotoPath,
                              int ProjectsCount,
                              string CreatorName,
                              DateTime CreatedDate,
                              int MembersCount,
                              int TasksCount);

public sealed record GetAllGroupDto(int Id,
                                    string Name,
                                    string Description,
                                    string PhotoPath);

public sealed record CreateGroupDto(string Name, string Description);

public sealed record UpdateGroupDto
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; init; }
    public string Description { get; init; }

    public UpdateGroupDto(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public sealed record GroupPhotoDto(int Id, string PhotoUrl);

public sealed record GroupMemberDto(int Id, string Username, string PictureUrl, int RoleId, string RoleName);

public sealed record CreateGroupMemberDto(int GroupId, string Username);

public sealed record UpdateGroupMemberDto(int Id, string Username);