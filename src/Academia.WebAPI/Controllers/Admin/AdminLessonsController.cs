using Academia.Application.Lessons.Commands.CreateLesson;
using Academia.Application.Lessons.Commands.DeleteLesson;
using Academia.Application.Lessons.Commands.UpdateLesson;
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
}

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
