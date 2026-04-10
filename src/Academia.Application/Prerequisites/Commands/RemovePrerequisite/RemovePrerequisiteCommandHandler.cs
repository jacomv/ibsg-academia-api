using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Prerequisites.Commands.RemovePrerequisite;

public class RemovePrerequisiteCommandHandler : IRequestHandler<RemovePrerequisiteCommand>
{
    private readonly IApplicationDbContext _context;

    public RemovePrerequisiteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemovePrerequisiteCommand request, CancellationToken cancellationToken)
    {
        var prereq = await _context.CoursePrerequisites
            .FirstOrDefaultAsync(p =>
                p.CourseId == request.CourseId &&
                p.PrerequisiteCourseId == request.PrerequisiteCourseId, cancellationToken);

        if (prereq is null)
            throw new NotFoundException("Prerequisite", $"{request.CourseId}->{request.PrerequisiteCourseId}");

        _context.CoursePrerequisites.Remove(prereq);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
