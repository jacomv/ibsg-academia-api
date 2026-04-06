using Academia.Application.Lessons.Queries.GetLessonById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Route("api/student/lessons")]
[Authorize]
public class StudentLessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentLessonsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLesson(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLessonByIdQuery(id), ct);
        return Ok(result);
    }
}
