using Academia.Application.Comments.Dtos;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Comments.Queries.GetLessonComments;

public class GetLessonCommentsQueryHandler : IRequestHandler<GetLessonCommentsQuery, List<CommentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLessonCommentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CommentDto>> Handle(
        GetLessonCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Where(c => c.LessonId == request.LessonId && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return comments.Select(c => MapToDto(c)).ToList();
    }

    private static CommentDto MapToDto(Domain.Entities.Comment c)
    {
        return new CommentDto(
            c.Id,
            c.LessonId,
            c.UserId,
            c.User?.FullName ?? "Unknown",
            c.User?.Avatar,
            c.Content,
            c.IsResolved,
            c.ParentCommentId,
            c.Replies.Select(r => MapToDto(r)).ToList(),
            c.CreatedAt);
    }
}
