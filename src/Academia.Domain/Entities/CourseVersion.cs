using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class CourseVersion : BaseEntity
{
    private CourseVersion() { }

    public CourseVersion(Guid courseId, string snapshotJson, Guid authorId, string reason)
    {
        CourseId = courseId;
        SnapshotJson = snapshotJson;
        AuthorId = authorId;
        Reason = reason;
        VersionNumber = 0; // Set by the handler after querying max
    }

    public Guid CourseId { get; private set; }
    public int VersionNumber { get; set; }
    public string SnapshotJson { get; private set; } = default!;
    public Guid AuthorId { get; private set; }
    public string Reason { get; private set; } = default!;

    public Course Course { get; private set; } = default!;
    public User Author { get; private set; } = default!;
}
