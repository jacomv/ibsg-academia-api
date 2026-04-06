using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;

namespace Academia.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Course(
            title: request.Title,
            description: request.Description,
            status: request.Status,
            accessType: request.AccessType,
            price: request.Price,
            estimatedDuration: request.EstimatedDuration,
            teacherId: request.TeacherId
        );

        if (request.Image is not null)
            course.SetImage(request.Image);

        await _context.Courses.AddAsync(course, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}
