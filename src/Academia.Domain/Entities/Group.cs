using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Group : BaseEntity
{
    private readonly List<GroupMember> _members = new();

    private Group() { }

    public Group(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
