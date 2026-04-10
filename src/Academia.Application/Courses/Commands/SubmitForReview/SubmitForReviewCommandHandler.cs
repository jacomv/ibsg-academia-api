using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.SubmitForReview;

public class SubmitForReviewCommandHandler : IRequestHandler<SubmitForReviewCommand>
{
    private readonly IApplicationDbContext _context;

    public SubmitForReviewCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(SubmitForReviewCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        course.SubmitForReview();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
