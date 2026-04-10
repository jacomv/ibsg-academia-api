using Academia.Application.Lessons.Commands.CreateLesson;
using Academia.Application.Lessons.Commands.DeleteLesson;
using Academia.Application.Lessons.Commands.ReorderLessons;
using Academia.Application.Lessons.Commands.UpdateLesson;
using Academia.Application.Lessons.Queries.GetLessonVersions;
using Academia.Application.Lessons.Queries.ValidateLesson;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminLessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminLessonsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("chapters/{chapterId:guid}/lessons")]
    public async Task<IActionResult> Create(
        Guid chapterId, [FromBody] LessonRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateLessonCommand(
            chapterId, request.Title, request.Type, request.TextContent,
            request.VideoUrl, request.AudioUrl, request.PdfFile,
            request.DurationMinutes, request.Order,
            request.RequiresCompletingPrevious, request.AvailableFrom), ct);

        return Created($"/api/admin/lessons/{id}", new { id });
    }

    [HttpPut("lessons/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] LessonRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateLessonCommand(
            id, request.Title, request.Type, request.TextContent,
            request.VideoUrl, request.AudioUrl, request.PdfFile,
            request.DurationMinutes, request.Order,
            request.RequiresCompletingPrevious, request.AvailableFrom), ct);

        return NoContent();
    }

    [HttpDelete("lessons/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteLessonCommand(id), ct);
        return NoContent();
    }

    [HttpPost("chapters/{chapterId:guid}/lessons/reorder")]
    public async Task<IActionResult> Reorder(
        Guid chapterId, [FromBody] LessonReorderRequest request, CancellationToken ct)
    {
        await _mediator.Send(new ReorderLessonsCommand(chapterId, request.OrderedIds), ct);
        return NoContent();
    }

    [HttpPost("lessons/{id:guid}/validate")]
    public async Task<IActionResult> Validate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ValidateLessonQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("lessons/{id:guid}/versions")]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLessonVersionsQuery(id), ct);
        return Ok(result);
    }
}

public record LessonReorderRequest(List<Guid> OrderedIds);

public record LessonRequest(
    string Title,
    LessonType Type,
    string? TextContent,
    string? VideoUrl,
    string? AudioUrl,
    string? PdfFile,
    int? DurationMinutes,
    int Order,
    bool RequiresCompletingPrevious,
    DateTime? AvailableFrom
);
