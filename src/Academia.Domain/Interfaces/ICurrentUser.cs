using Academia.Domain.Enums;

namespace Academia.Domain.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin => Role == UserRole.Administrator;
    bool IsTeacher => Role == UserRole.Teacher;
    bool IsStudent => Role == UserRole.Student;
}
