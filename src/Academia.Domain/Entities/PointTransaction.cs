using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class PointTransaction : BaseEntity
{
    private PointTransaction() { }

    public PointTransaction(Guid userId, int points, string reason, string? referenceId = null)
    {
        UserId = userId;
        Points = points;
        Reason = reason;
        ReferenceId = referenceId;
    }

    public Guid UserId { get; private set; }
    public int Points { get; private set; }
    public string Reason { get; private set; } = default!;
    public string? ReferenceId { get; private set; }

    public User User { get; private set; } = default!;
}
