using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class GroupMember : BaseEntity
{
    private GroupMember() { }

    public GroupMember(Guid groupId, Guid userId)
    {
        GroupId = groupId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    public Guid GroupId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Group Group { get; private set; } = default!;
    public User User { get; private set; } = default!;
}
