using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Lessons.Validators;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Commands.CreateLesson;

public class CreateLessonCommandHandler : IRequestHandler<CreateLessonCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLessonCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var chapterExists = await _context.Chapters
            .AnyAsync(c => c.Id == request.ChapterId, cancellationToken);

        if (!chapterExists)
            throw new NotFoundException("Chapter", request.ChapterId);

        var validation = LessonContentValidator.Validate(
            request.Type, request.TextContent, request.VideoUrl,
            request.AudioUrl, request.PdfFile);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var lesson = new Lesson(
            request.ChapterId, request.Title, request.Type,
            request.Order, request.RequiresCompletingPrevious);

        lesson.Update(
            request.Title, request.Type, request.TextContent, request.VideoUrl,
            request.AudioUrl, request.PdfFile, request.DurationMinutes,
            request.Order, request.RequiresCompletingPrevious, request.AvailableFrom);

        await _context.Lessons.AddAsync(lesson, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return lesson.Id;
    }
}
