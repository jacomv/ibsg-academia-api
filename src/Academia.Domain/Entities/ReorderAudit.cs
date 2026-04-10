using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class ReorderAudit : BaseEntity
{
    private ReorderAudit() { }

    public ReorderAudit(string entityType, Guid parentId, Guid userId, string previousOrderJson, string newOrderJson)
    {
        EntityType = entityType;
        ParentId = parentId;
        UserId = userId;
        PreviousOrderJson = previousOrderJson;
        NewOrderJson = newOrderJson;
    }

    /// <summary>"Chapter" or "Lesson"</summary>
    public string EntityType { get; private set; } = default!;
    public Guid ParentId { get; private set; }
    public Guid UserId { get; private set; }
    public string PreviousOrderJson { get; private set; } = default!;
    public string NewOrderJson { get; private set; } = default!;

    public User User { get; private set; } = default!;
}
