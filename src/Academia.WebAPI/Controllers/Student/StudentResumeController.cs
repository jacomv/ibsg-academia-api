using Academia.Application.Progress.Commands.CompleteLesson;
using Academia.Application.Student.Queries.GetResume;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Route("api/student")]
[Authorize]
public class StudentResumeController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentResumeController(IMediator mediator) => _mediator = mediator;

    [HttpGet("resume")]
    public async Task<IActionResult> GetResume(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetResumeQuery(), ct);
        if (result is null)
            return NoContent();
        return Ok(result);
    }

    [HttpPost("lessons/{lessonId:guid}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId, CancellationToken ct)
    {
        var result = await _mediator.Send(new CompleteLessonCommand(lessonId), ct);
        return Ok(result);
    }
}
