using Academia.Application.Admin.Dtos;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Admin.Queries.GetDashboard;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminDashboardQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AdminDashboardDto> Handle(
        GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        // Sequential queries — EF Core DbContext is not thread-safe
        var publishedCourses = await _context.Courses
            .CountAsync(c => c.Status == CourseStatus.Published, cancellationToken);
        var draftCourses = await _context.Courses
            .CountAsync(c => c.Status == CourseStatus.Draft, cancellationToken);
        var totalStudents = await _context.Users
            .CountAsync(u => u.Role == UserRole.Student && u.IsActive, cancellationToken);
        var totalTeachers = await _context.Users
            .CountAsync(u => u.Role == UserRole.Teacher && u.IsActive, cancellationToken);
        var activeLearningPaths = await _context.LearningPaths
            .CountAsync(lp => lp.Status == CourseStatus.Published, cancellationToken);
        var pendingGrades = await _context.Grades
            .CountAsync(g => g.Status == GradingStatus.Pending, cancellationToken);
        var activeEnrollments = await _context.Enrollments
            .CountAsync(e => e.Status == EnrollmentStatus.Active, cancellationToken);

        // Recent enrollments (last 10)
        var recentEnrollments = await _context.Enrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrolledAt)
            .Take(10)
            .AsNoTracking()
            .Select(e => new RecentEnrollmentDto(
                e.Id,
                e.User.FirstName + " " + e.User.LastName,
                e.User.Email,
                e.Course.Title,
                e.Status.ToString(),
                e.EnrolledAt))
            .ToListAsync(cancellationToken);

        // Top 5 courses by active enrollments — group enrollments first, then join
        var enrollmentCounts = await _context.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Active)
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CourseId, x => x.Count, cancellationToken);

        var topCourses = await _context.Courses
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.Id)
            .Take(20)
            .AsNoTracking()
            .Select(c => new { c.Id, c.Title })
            .ToListAsync(cancellationToken);

        var topCourseDtos = topCourses
            .Select(c => new CourseEnrollmentStatsDto(
                c.Id, c.Title,
                enrollmentCounts.GetValueOrDefault(c.Id, 0), 0, 0m))
            .OrderByDescending(c => c.ActiveEnrollments)
            .Take(5)
            .ToList();

        return new AdminDashboardDto(
            PublishedCourses: publishedCourses,
            DraftCourses: draftCourses,
            TotalStudents: totalStudents,
            TotalTeachers: totalTeachers,
            ActiveLearningPaths: activeLearningPaths,
            PendingGrades: pendingGrades,
            ActiveEnrollments: activeEnrollments,
            RecentEnrollments: recentEnrollments,
            TopCourses: topCourseDtos
        );
    }
}
