using Academia.Application.Comments.Dtos;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using Academia.Domain.Entities;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IMediator _mediator;

    public CreateCommentCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var lessonExists = await _context.Lessons
            .AnyAsync(l => l.Id == request.LessonId, cancellationToken);
        if (!lessonExists)
            throw new NotFoundException("Lesson", request.LessonId);

        if (request.ParentCommentId.HasValue)
        {
            var parentExists = await _context.Comments
                .AnyAsync(c => c.Id == request.ParentCommentId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException("Comment", request.ParentCommentId.Value);
        }

        var comment = new Comment(
            request.LessonId,
            _currentUser.Id,
            request.Content,
            request.ParentCommentId);

        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish event to notify teacher/admin
        await _mediator.Publish(new CommentCreatedEvent(
            comment.Id,
            comment.LessonId,
            comment.UserId,
            _currentUser.Email),
            cancellationToken);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.Id, cancellationToken);

        return new CommentDto(
            comment.Id,
            comment.LessonId,
            comment.UserId,
            user?.FullName ?? _currentUser.Email,
            user?.Avatar,
            comment.Content,
            comment.IsResolved,
            comment.ParentCommentId,
            new List<CommentDto>(),
            comment.CreatedAt);
    }
}
