using Academia.Application.Grading.Commands.ManualGrade;
using Academia.Application.Grading.Dtos;
using Academia.Application.Grading.Queries.GetGradeDetail;
using Academia.Application.Grading.Queries.GetPendingGrades;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Teacher;

[ApiController]
[Route("api/teacher/grades")]
[Authorize(Policy = "AdminOrTeacher")]
public class TeacherGradingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeacherGradingController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetGrades(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] GradingStatus? status = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPendingGradesQuery(page, pageSize, status), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGradeDetailQuery(id), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Grade(
        Guid id, [FromBody] ManualGradeBody body, CancellationToken ct)
    {
        await _mediator.Send(new ManualGradeCommand(id, body.Scores, body.Feedback), ct);
        return NoContent();
    }
}

public record ManualGradeBody(List<ManualScoreInput> Scores, string? Feedback);
