using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class User : BaseEntity
{
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<Notification> _notifications = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    private User() { }

    public User(string firstName, string lastName, string email, string passwordHash, UserRole role)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public string? Avatar { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName, string? avatar)
    {
        FirstName = firstName;
        LastName = lastName;
        Avatar = avatar;
    }

    public void ChangePassword(string newPasswordHash) => PasswordHash = newPasswordHash;
    public void ChangeRole(UserRole role) => Role = role;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
