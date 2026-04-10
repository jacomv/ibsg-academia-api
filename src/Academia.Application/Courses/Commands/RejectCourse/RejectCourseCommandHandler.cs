using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.RejectCourse;

public class RejectCourseCommandHandler : IRequestHandler<RejectCourseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RejectCourseCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RejectCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        course.ReturnToDraft();

        var review = new EditorialReview(
            request.CourseId, _currentUser.Id, EditorialDecision.Rejected, request.Comment);
        await _context.EditorialReviews.AddAsync(review, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
