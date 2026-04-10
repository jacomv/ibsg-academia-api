using Academia.Application.Attachments.Commands.CreateAttachment;
using Academia.Application.Attachments.Commands.DeleteAttachment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminAttachmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminAttachmentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("lessons/{lessonId:guid}/attachments")]
    public async Task<IActionResult> Create(
        Guid lessonId, [FromBody] CreateAttachmentRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateAttachmentCommand(
            lessonId, request.FileName, request.FileUrl,
            request.FileType, request.FileSize, request.Order), ct);
        return Created($"/api/admin/attachments/{id}", new { id });
    }

    [HttpDelete("attachments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAttachmentCommand(id), ct);
        return NoContent();
    }
}

public record CreateAttachmentRequest(
    string FileName,
    string FileUrl,
    string FileType,
    long FileSize,
    int Order
);
