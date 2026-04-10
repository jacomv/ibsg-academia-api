using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Attachments.Commands.DeleteAttachment;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAttachmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.LessonAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId, cancellationToken);

        if (attachment is null)
            throw new NotFoundException("Attachment", request.AttachmentId);

        _context.LessonAttachments.Remove(attachment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
