using MediatR;

namespace Academia.Application.Comments.Commands.ResolveComment;

public record ResolveCommentCommand(Guid CommentId) : IRequest;
