using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.ScheduleCourse;

public class ScheduleCourseCommandHandler : IRequestHandler<ScheduleCourseCommand>
{
    private readonly IApplicationDbContext _context;

    public ScheduleCourseCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ScheduleCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        if (request.PublishAt <= DateTime.UtcNow)
            throw new ValidationException(new[] { "Publish date must be in the future." });

        if (request.UnpublishAt.HasValue && request.UnpublishAt <= request.PublishAt)
            throw new ValidationException(new[] { "Unpublish date must be after publish date." });

        course.Schedule(request.PublishAt, request.UnpublishAt);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
