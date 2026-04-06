using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CancelEnrollmentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
            throw new NotFoundException("Enrollment", request.EnrollmentId);

        // Students can only cancel their own enrollments
        if (_currentUser.IsStudent && enrollment.UserId != _currentUser.Id)
            throw new UnauthorizedException("You can only cancel your own enrollments.");

        enrollment.Cancel();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
