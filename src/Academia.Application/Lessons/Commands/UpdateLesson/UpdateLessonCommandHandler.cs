using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Commands.UpdateLesson;

public class UpdateLessonCommandHandler : IRequestHandler<UpdateLessonCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateLessonCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.Id);

        lesson.Update(
            request.Title, request.Type, request.TextContent, request.VideoUrl,
            request.AudioUrl, request.PdfFile, request.DurationMinutes,
            request.Order, request.RequiresCompletingPrevious, request.AvailableFrom);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
