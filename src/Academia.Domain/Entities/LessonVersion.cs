using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class LessonVersion : BaseEntity
{
    private LessonVersion() { }

    public LessonVersion(Guid lessonId, string snapshotJson, Guid authorId, string reason)
    {
        LessonId = lessonId;
        SnapshotJson = snapshotJson;
        AuthorId = authorId;
        Reason = reason;
        VersionNumber = 0; // Set by the handler after querying max
    }

    public Guid LessonId { get; private set; }
    public int VersionNumber { get; set; }
    public string SnapshotJson { get; private set; } = default!;
    public Guid AuthorId { get; private set; }
    public string Reason { get; private set; } = default!;

    public Lesson Lesson { get; private set; } = default!;
    public User Author { get; private set; } = default!;
}
