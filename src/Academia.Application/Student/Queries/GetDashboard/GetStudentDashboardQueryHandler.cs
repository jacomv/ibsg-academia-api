using Academia.Application.Common.Interfaces;
using Academia.Application.Student.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Student.Queries.GetDashboard;

public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, StudentDashboardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetStudentDashboardQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<StudentDashboardDto> Handle(
        GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;

        // All active enrollments with course data
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Chapters)
                    .ThenInclude(ch => ch.Lessons)
            .Where(e => e.UserId == userId && e.Status == EnrollmentStatus.Active)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
        var allLessonIds = enrollments
            .SelectMany(e => e.Course.Chapters.SelectMany(ch => ch.Lessons.Select(l => l.Id)))
            .ToList();

        // All progress for enrolled courses
        var progressRecords = await _context.UserProgress
            .Where(p => p.UserId == userId && allLessonIds.Contains(p.LessonId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var completedCount = progressRecords.Count(p => p.Status == ProgressStatus.Completed);
        var totalLessons = allLessonIds.Count;

        // Recent grades
        var recentGrades = await _context.Grades
            .Include(g => g.Exam)
                .ThenInclude(e => e.Course)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Chapter)
                    .ThenInclude(ch => ch != null ? ch.Course : null)
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .Take(5)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var avgScore = recentGrades.Count > 0
            ? Math.Round(recentGrades.Average(g => g.TotalScore), 1)
            : 0m;

        // Unread notifications
        var unreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

        // Build per-course progress summary
        var coursesInProgress = enrollments
            .Select(e =>
            {
                var courseLessonIds = e.Course.Chapters
                    .SelectMany(ch => ch.Lessons.Select(l => l.Id))
                    .ToHashSet();

                var courseCompleted = progressRecords
                    .Count(p => courseLessonIds.Contains(p.LessonId) &&
                                p.Status == ProgressStatus.Completed);

                var courseTotal = courseLessonIds.Count;
                var pct = courseTotal > 0
                    ? Math.Round((decimal)courseCompleted / courseTotal * 100, 1)
                    : 0m;

                return new CourseInProgressDto(
                    e.CourseId, e.Course.Title, e.Course.Image,
                    courseCompleted, courseTotal, pct);
            })
            .Where(c => c.ProgressPercentage < 100)
            .OrderByDescending(c => c.ProgressPercentage)
            .Take(5)
            .ToList();

        var recentGradeDtos = recentGrades.Select(g =>
        {
            var courseTitle = g.Exam.Course?.Title
                ?? g.Exam.Chapter?.Course?.Title
                ?? "—";
            return new RecentGradeDto(
                g.Id, g.Exam.Title, courseTitle,
                g.TotalScore, g.IsPassed, g.CreatedAt);
        }).ToList();

        return new StudentDashboardDto(
            ActiveEnrollments: enrollments.Count,
            CompletedLessons: completedCount,
            TotalLessons: totalLessons,
            OverallProgress: totalLessons > 0
                ? Math.Round((decimal)completedCount / totalLessons * 100, 1) : 0m,
            AverageScore: avgScore,
            UnreadNotifications: unreadCount,
            CoursesInProgress: coursesInProgress,
            RecentGrades: recentGradeDtos);
    }
}
