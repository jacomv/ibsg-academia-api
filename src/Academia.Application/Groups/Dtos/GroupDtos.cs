namespace Academia.Application.Groups.Dtos;

public record GroupListDto(
    Guid Id,
    string Name,
    string? Description,
    int MemberCount,
    DateTime CreatedAt
);

public record GroupDetailDto(
    Guid Id,
    string Name,
    string? Description,
    List<GroupMemberDto> Members,
    DateTime CreatedAt
);

public record GroupMemberDto(
    Guid UserId,
    string FullName,
    string Email,
    DateTime JoinedAt
);
