using MediatR;

namespace Academia.Application.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    Guid CommentId,
    string Content
) : IRequest;
