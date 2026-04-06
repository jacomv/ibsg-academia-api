using Academia.Application.Enrollments.Commands.CancelEnrollment;
using Academia.Application.Enrollments.Commands.SelfEnroll;
using Academia.Application.Enrollments.Queries.GetMyEnrollments;
using Academia.Application.Student.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Authorize]
public class StudentEnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentEnrollmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("api/student/dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetStudentDashboardQuery(), ct);
        return Ok(result);
    }

    [HttpGet("api/student/enrollments")]
    public async Task<IActionResult> MyEnrollments(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyEnrollmentsQuery(), ct);
        return Ok(result);
    }

    [HttpPost("api/enrollments")]
    public async Task<IActionResult> Enroll([FromBody] SelfEnrollCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(MyEnrollments), new { }, result);
    }

    [HttpDelete("api/enrollments/{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CancelEnrollmentCommand(id), ct);
        return NoContent();
    }
}
