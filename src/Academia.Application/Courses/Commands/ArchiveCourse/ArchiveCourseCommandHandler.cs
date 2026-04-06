using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.ArchiveCourse;

public class ArchiveCourseCommandHandler : IRequestHandler<ArchiveCourseCommand>
{
    private readonly IApplicationDbContext _context;

    public ArchiveCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.Id);

        course.Archive();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
