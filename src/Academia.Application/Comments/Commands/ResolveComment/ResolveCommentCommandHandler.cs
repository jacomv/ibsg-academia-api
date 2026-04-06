using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Comments.Commands.ResolveComment;

public class ResolveCommentCommandHandler : IRequestHandler<ResolveCommentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ResolveCommentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ResolveCommentCommand request, CancellationToken cancellationToken)
    {
        bool isTeacherOrAdmin =
            _currentUser.Role == UserRole.Teacher ||
            _currentUser.Role == UserRole.Administrator;

        if (!isTeacherOrAdmin)
            throw new UnauthorizedException("Only teachers and admins can resolve comments.");

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

        if (comment is null)
            throw new NotFoundException("Comment", request.CommentId);

        comment.Resolve();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
