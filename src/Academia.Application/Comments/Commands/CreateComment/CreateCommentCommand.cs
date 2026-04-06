using Academia.Application.Comments.Dtos;
using MediatR;

namespace Academia.Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    Guid LessonId,
    string Content,
    Guid? ParentCommentId
) : IRequest<CommentDto>;
