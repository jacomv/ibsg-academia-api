using System.Text.Json;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.PublishCourse;

public class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public PublishCourseCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Chapters)
                .ThenInclude(ch => ch.Lessons)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.Approved)
            throw new InvalidOperationException("Only approved courses can be published.");

        // Create version snapshot before publishing
        var maxVersion = await _context.CourseVersions
            .Where(v => v.CourseId == course.Id)
            .MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0;

        var snapshot = JsonSerializer.Serialize(new
        {
            course.Title,
            course.Description,
            course.Image,
            course.Status,
            course.AccessType,
            course.Price,
            course.EstimatedDuration,
            Chapters = course.Chapters.Select(ch => new
            {
                ch.Id, ch.Title, ch.Description, ch.Order,
                Lessons = ch.Lessons.Select(l => new
                {
                    l.Id, l.Title, l.Type, l.Order, l.TextContent,
                    l.VideoUrl, l.AudioUrl, l.PdfFile, l.DurationMinutes
                })
            })
        });

        var version = new CourseVersion(course.Id, snapshot, _currentUser.Id, "Published");
        version.VersionNumber = maxVersion + 1;
        await _context.CourseVersions.AddAsync(version, cancellationToken);

        course.Publish();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
