namespace Academia.Application.Comments.Dtos;

public record CommentDto(
    Guid Id,
    Guid LessonId,
    Guid UserId,
    string AuthorName,
    string? AuthorAvatar,
    string Content,
    bool IsResolved,
    Guid? ParentCommentId,
    List<CommentDto> Replies,
    DateTime CreatedAt
);
