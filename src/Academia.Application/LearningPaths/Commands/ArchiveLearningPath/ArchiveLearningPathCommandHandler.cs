using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Commands.ArchiveLearningPath;

public class ArchiveLearningPathCommandHandler : IRequestHandler<ArchiveLearningPathCommand>
{
    private readonly IApplicationDbContext _context;

    public ArchiveLearningPathCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ArchiveLearningPathCommand request, CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .FirstOrDefaultAsync(lp => lp.Id == request.Id, cancellationToken);

        if (path is null) throw new NotFoundException("LearningPath", request.Id);

        path.Archive();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
