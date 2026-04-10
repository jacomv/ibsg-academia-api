using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.ApproveCourse;

public class ApproveCourseCommandHandler : IRequestHandler<ApproveCourseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ApproveCourseCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ApproveCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        course.Approve();

        var review = new EditorialReview(
            request.CourseId, _currentUser.Id, EditorialDecision.Approved, request.Comment);
        await _context.EditorialReviews.AddAsync(review, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
