using MediatR;

namespace Academia.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : IRequest;
