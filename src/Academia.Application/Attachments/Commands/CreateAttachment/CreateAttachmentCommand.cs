using MediatR;

namespace Academia.Application.Attachments.Commands.CreateAttachment;

public record CreateAttachmentCommand(
    Guid LessonId,
    string FileName,
    string FileUrl,
    string FileType,
    long FileSize,
    int Order
) : IRequest<Guid>;
