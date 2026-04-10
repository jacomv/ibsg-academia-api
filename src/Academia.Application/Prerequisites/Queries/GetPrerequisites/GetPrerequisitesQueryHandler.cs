using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Prerequisites.Queries.GetPrerequisites;

public class GetPrerequisitesQueryHandler : IRequestHandler<GetPrerequisitesQuery, List<PrerequisiteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetPrerequisitesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PrerequisiteDto>> Handle(
        GetPrerequisitesQuery request, CancellationToken cancellationToken)
    {
        var prerequisites = await _context.CoursePrerequisites
            .Where(p => p.CourseId == request.CourseId)
            .Include(p => p.PrerequisiteCourse)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<PrerequisiteDto>();

        foreach (var p in prerequisites)
        {
            // Check if current user completed this prerequisite course
            var totalLessons = await _context.Lessons
                .CountAsync(l =>
                    l.Chapter.CourseId == p.PrerequisiteCourseId &&
                    l.Type != LessonType.Section, cancellationToken);

            var completedLessons = 0;
            if (totalLessons > 0)
            {
                completedLessons = await _context.UserProgress
                    .CountAsync(up =>
                        up.UserId == _currentUser.Id &&
                        up.Status == ProgressStatus.Completed &&
                        _context.Lessons.Any(l =>
                            l.Id == up.LessonId &&
                            l.Type != LessonType.Section &&
                            l.Chapter.CourseId == p.PrerequisiteCourseId), cancellationToken);
            }

            result.Add(new PrerequisiteDto(
                p.PrerequisiteCourseId,
                p.PrerequisiteCourse.Title,
                p.PrerequisiteCourse.Image,
                totalLessons > 0 && completedLessons >= totalLessons));
        }

        return result;
    }
}
