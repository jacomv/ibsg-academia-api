using Academia.Domain.Enums;

namespace Academia.Application.Users.Dtos;

public record UserListDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt,
    int ActiveEnrollments
);
