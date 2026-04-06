using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Comment : BaseEntity
{
    private readonly List<Comment> _replies = new();

    private Comment() { }

    public Comment(Guid lessonId, Guid userId, string content, Guid? parentCommentId = null)
    {
        LessonId = lessonId;
        UserId = userId;
        Content = content;
        ParentCommentId = parentCommentId;
        IsResolved = false;
    }

    public Guid LessonId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = default!;
    public Guid? ParentCommentId { get; private set; }
    public bool IsResolved { get; private set; }

    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;
    public Comment? ParentComment { get; private set; }
    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

    public void Resolve() => IsResolved = true;
    public void UpdateContent(string content) => Content = content;
}
