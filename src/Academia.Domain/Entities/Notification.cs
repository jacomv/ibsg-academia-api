using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Notification : BaseEntity
{
    private Notification() { }

    public Notification(Guid userId, string type, string title, string message)
    {
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
        IsRead = false;
    }

    public Guid UserId { get; private set; }
    public string Type { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public User User { get; private set; } = default!;

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}
