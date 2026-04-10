using Academia.Application.Chapters.Commands.CreateChapter;
using Academia.Application.Chapters.Commands.DeleteChapter;
using Academia.Application.Chapters.Commands.ReorderChapters;
using Academia.Application.Chapters.Commands.UpdateChapter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminChaptersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminChaptersController(IMediator mediator) => _mediator = mediator;

    [HttpPost("courses/{courseId:guid}/chapters")]
    public async Task<IActionResult> Create(
        Guid courseId, [FromBody] CreateChapterRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateChapterCommand(
            courseId, request.Title, request.Description, request.Order, request.AvailableFrom), ct);
        return Created($"/api/admin/chapters/{id}", new { id });
    }

    [HttpPut("chapters/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateChapterRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateChapterCommand(
            id, request.Title, request.Description, request.Order, request.AvailableFrom), ct);
        return NoContent();
    }

    [HttpDelete("chapters/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteChapterCommand(id), ct);
        return NoContent();
    }

    [HttpPost("courses/{courseId:guid}/chapters/reorder")]
    public async Task<IActionResult> Reorder(
        Guid courseId, [FromBody] ReorderRequest request, CancellationToken ct)
    {
        await _mediator.Send(new ReorderChaptersCommand(courseId, request.OrderedIds), ct);
        return NoContent();
    }
}

public record ReorderRequest(List<Guid> OrderedIds);

public record CreateChapterRequest(
    string Title, string? Description, int Order, DateTime? AvailableFrom);

public record UpdateChapterRequest(
    string Title, string? Description, int Order, DateTime? AvailableFrom);
