using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Commands.UpdateLearningPath;

public class UpdateLearningPathCommandHandler : IRequestHandler<UpdateLearningPathCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateLearningPathCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateLearningPathCommand request, CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .FirstOrDefaultAsync(lp => lp.Id == request.Id, cancellationToken);

        if (path is null) throw new NotFoundException("LearningPath", request.Id);

        path.Update(request.Name, request.Description, request.Image,
            request.Status, request.AccessType, request.Price, request.GlobalOrder);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
