using Academia.Application.Common.Interfaces;
using Academia.Application.Common.Models;
using Academia.Application.Grading.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Grading.Queries.GetPendingGrades;

public class GetPendingGradesQueryHandler
    : IRequestHandler<GetPendingGradesQuery, PagedResult<PendingGradeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetPendingGradesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<PendingGradeDto>> Handle(
        GetPendingGradesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Grades
            .Include(g => g.User)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Course)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Chapter)
                    .ThenInclude(ch => ch != null ? ch.Course : null)
            .AsNoTracking()
            .AsQueryable();

        // Teachers only see grades for their own courses
        if (_currentUser.IsTeacher)
        {
            query = query.Where(g =>
                (g.Exam.Course != null && g.Exam.Course.TeacherId == _currentUser.Id) ||
                (g.Exam.Chapter != null && g.Exam.Chapter.Course.TeacherId == _currentUser.Id));
        }

        if (request.Status.HasValue)
            query = query.Where(g => g.Status == request.Status.Value);
        else
            query = query.Where(g => g.Status == GradingStatus.Pending);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new PendingGradeDto(
                g.Id,
                g.ExamId,
                g.Exam.Title,
                g.Exam.Course != null ? g.Exam.Course.Title
                    : g.Exam.Chapter != null ? g.Exam.Chapter.Course.Title : "—",
                g.UserId,
                g.User.FirstName + " " + g.User.LastName,
                g.User.Email,
                g.AttemptNumber,
                g.TotalScore,
                g.Status,
                g.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<PendingGradeDto>(items, totalCount, request.Page, request.PageSize);
    }
}
