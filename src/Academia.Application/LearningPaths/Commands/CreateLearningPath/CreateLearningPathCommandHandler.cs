using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;

namespace Academia.Application.LearningPaths.Commands.CreateLearningPath;

public class CreateLearningPathCommandHandler : IRequestHandler<CreateLearningPathCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLearningPathCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateLearningPathCommand request, CancellationToken cancellationToken)
    {
        var path = new LearningPath(
            request.Name, request.Description, request.Status,
            request.AccessType, request.Price, request.GlobalOrder);

        if (request.Image is not null)
            path.Update(request.Name, request.Description, request.Image,
                request.Status, request.AccessType, request.Price, request.GlobalOrder);

        await _context.LearningPaths.AddAsync(path, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return path.Id;
    }
}
