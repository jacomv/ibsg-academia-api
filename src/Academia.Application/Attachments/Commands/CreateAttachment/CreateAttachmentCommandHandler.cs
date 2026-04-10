using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Attachments.Commands.CreateAttachment;

public class CreateAttachmentCommandHandler : IRequestHandler<CreateAttachmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateAttachmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateAttachmentCommand request, CancellationToken cancellationToken)
    {
        var lessonExists = await _context.Lessons
            .AnyAsync(l => l.Id == request.LessonId, cancellationToken);

        if (!lessonExists)
            throw new NotFoundException("Lesson", request.LessonId);

        var attachment = new LessonAttachment(
            request.LessonId, request.FileName, request.FileUrl,
            request.FileType, request.FileSize, request.Order);

        await _context.LessonAttachments.AddAsync(attachment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return attachment.Id;
    }
}
