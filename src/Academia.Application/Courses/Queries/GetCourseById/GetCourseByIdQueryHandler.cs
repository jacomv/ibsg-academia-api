using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Courses.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetCourseByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseDetailDto> Handle(
        GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Chapters.OrderBy(ch => ch.Order))
                .ThenInclude(ch => ch.Lessons.OrderBy(l => l.Order))
            .Include(c => c.Chapters)
                .ThenInclude(ch => ch.Exam)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.Id);

        return new CourseDetailDto(
            Id: course.Id,
            Title: course.Title,
            Description: course.Description,
            Image: course.Image,
            Status: course.Status,
            AccessType: course.AccessType,
            Price: course.Price,
            EstimatedDuration: course.EstimatedDuration,
            Teacher: course.Teacher is null ? null
                : new TeacherSummaryDto(course.Teacher.Id, course.Teacher.FullName, course.Teacher.Avatar),
            Chapters: course.Chapters.Select(ch => new ChapterDto(
                Id: ch.Id,
                Title: ch.Title,
                Description: ch.Description,
                Order: ch.Order,
                AvailableFrom: ch.AvailableFrom,
                IsLocked: ch.IsLocked,
                Lessons: ch.Lessons.Select(l => new LessonSummaryDto(
                    Id: l.Id,
                    Title: l.Title,
                    Type: l.Type,
                    Order: l.Order,
                    DurationMinutes: l.DurationMinutes,
                    RequiresCompletingPrevious: l.RequiresCompletingPrevious,
                    IsLockedByDate: l.IsLockedByDate
                )).ToList(),
                Exam: ch.Exam is null ? null
                    : new ExamSummaryDto(ch.Exam.Id, ch.Exam.Title, ch.Exam.PassingScore, ch.Exam.MaxAttempts),
                EstimatedDurationMinutes: ch.Lessons
                    .Where(l => l.DurationMinutes.HasValue)
                    .Sum(l => l.DurationMinutes!.Value)
            )).ToList(),
            CreatedAt: course.CreatedAt
        );
    }
}
