using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class LessonAttachment : BaseEntity
{
    private LessonAttachment() { }

    public LessonAttachment(Guid lessonId, string fileName, string fileUrl, string fileType, long fileSize, int order)
    {
        LessonId = lessonId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileType = fileType;
        FileSize = fileSize;
        Order = order;
    }

    public Guid LessonId { get; private set; }
    public string FileName { get; private set; } = default!;
    public string FileUrl { get; private set; } = default!;
    public string FileType { get; private set; } = default!;
    public long FileSize { get; private set; }
    public int Order { get; private set; }

    public Lesson Lesson { get; private set; } = default!;
}
