using Academia.Application.Admin.Dtos;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<UserProfileDto> Handle(
        GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Enrollments)
                .ThenInclude(e => e.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        var recentGrades = await _context.Grades
            .Include(g => g.Exam)
            .Where(g => g.UserId == request.UserId)
            .OrderByDescending(g => g.CreatedAt)
            .Take(10)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var activeEnrollments = user.Enrollments.Count(e => e.Status == EnrollmentStatus.Active);
        var completedExams = recentGrades.Count(g => g.Status != GradingStatus.Pending);
        var avgScore = completedExams > 0
            ? Math.Round(recentGrades.Where(g => g.Status != GradingStatus.Pending)
                .Average(g => g.TotalScore), 1)
            : 0m;

        return new UserProfileDto(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Role: user.Role.ToString(),
            Avatar: user.Avatar,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt,
            ActiveEnrollments: activeEnrollments,
            CompletedExams: completedExams,
            AverageScore: avgScore,
            Enrollments: user.Enrollments
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new EnrollmentSummaryDto(
                    e.CourseId, e.Course.Title, e.Status.ToString(), e.EnrolledAt))
                .ToList(),
            RecentGrades: recentGrades.Select(g => new GradeSummaryDto(
                g.Exam.Title, g.TotalScore, g.IsPassed, g.CreatedAt)).ToList()
        );
    }
}
