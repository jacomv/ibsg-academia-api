using MediatR;

namespace Academia.Application.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid AttachmentId) : IRequest;
