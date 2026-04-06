using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    private PasswordResetToken() { }

    public PasswordResetToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public User User { get; private set; } = default!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;

    public void MarkUsed() => IsUsed = true;
}
