using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class Lesson : BaseEntity
{
    private Lesson() { }

    public Lesson(Guid chapterId, string title, LessonType type, int order, bool requiresCompletingPrevious)
    {
        ChapterId = chapterId;
        Title = title;
        Type = type;
        Order = order;
        RequiresCompletingPrevious = requiresCompletingPrevious;
    }

    public Guid ChapterId { get; private set; }
    public string Title { get; private set; } = default!;
    public LessonType Type { get; private set; }
    public string? TextContent { get; private set; }
    public string? VideoUrl { get; private set; }
    public string? AudioUrl { get; private set; }
    public string? PdfFile { get; private set; }
    public int? DurationMinutes { get; private set; }
    public int Order { get; private set; }
    public bool RequiresCompletingPrevious { get; private set; }
    public DateTime? AvailableFrom { get; private set; }

    public Chapter Chapter { get; private set; } = default!;

    public bool IsLockedByDate => AvailableFrom.HasValue && DateTime.UtcNow < AvailableFrom.Value;

    public void Update(string title, LessonType type, string? textContent, string? videoUrl,
        string? audioUrl, string? pdfFile, int? durationMinutes, int order,
        bool requiresCompletingPrevious, DateTime? availableFrom)
    {
        Title = title;
        Type = type;
        TextContent = textContent;
        VideoUrl = videoUrl;
        AudioUrl = audioUrl;
        PdfFile = pdfFile;
        DurationMinutes = durationMinutes;
        Order = order;
        RequiresCompletingPrevious = requiresCompletingPrevious;
        AvailableFrom = availableFrom;
    }
}
