using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Commands.ActivateEnrollment;

public class ActivateEnrollmentCommandHandler : IRequestHandler<ActivateEnrollmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPublisher _publisher;

    public ActivateEnrollmentCommandHandler(IApplicationDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task Handle(ActivateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
            throw new NotFoundException("Enrollment", request.EnrollmentId);

        enrollment.Activate();
        await _context.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(
            new EnrollmentActivatedEvent(enrollment.UserId, enrollment.CourseId, enrollment.Course.Title),
            cancellationToken);
    }
}
