using Academia.Application.Progress.Commands.UpsertProgress;
using Academia.Application.Progress.Queries.GetCourseProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Authorize]
public class StudentProgressController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentProgressController(IMediator mediator) => _mediator = mediator;

    [HttpGet("api/student/courses/{courseId:guid}/progress")]
    public async Task<IActionResult> GetCourseProgress(Guid courseId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCourseProgressQuery(courseId), ct);
        return Ok(result);
    }

    [HttpPatch("api/progress/{lessonId:guid}")]
    public async Task<IActionResult> UpsertProgress(
        Guid lessonId,
        [FromBody] UpsertProgressBody body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpsertProgressCommand(
            lessonId, body.Status, body.VideoPosition,
            body.AudioPosition, body.ProgressPercentage), ct);
        return Ok(result);
    }
}

public record UpsertProgressBody(
    Academia.Domain.Enums.ProgressStatus Status,
    int? VideoPosition,
    int? AudioPosition,
    decimal ProgressPercentage
);
