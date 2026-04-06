namespace Academia.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}

public record EmailMessage(
    string To,
    string ToName,
    string Subject,
    string HtmlBody,
    string? PlainTextBody = null,
    byte[]? Attachment = null,
    string? AttachmentFileName = null
);
