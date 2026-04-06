using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteCommentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

        if (comment is null)
            throw new NotFoundException("Comment", request.CommentId);

        bool isAuthor = comment.UserId == _currentUser.Id;
        bool isAdmin = _currentUser.Role == UserRole.Administrator;

        if (!isAuthor && !isAdmin)
            throw new UnauthorizedException("You can only delete your own comments.");

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
