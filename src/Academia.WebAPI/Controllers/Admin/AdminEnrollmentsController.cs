using Academia.Application.Enrollments.Commands.ActivateEnrollment;
using Academia.Application.Enrollments.Commands.ForceEnroll;
using Academia.Application.Enrollments.Queries.GetAllEnrollments;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminEnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminEnrollmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("enrollments")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] EnrollmentStatus? status = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? courseId = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAllEnrollmentsQuery(page, pageSize, status, userId, courseId), ct);
        return Ok(result);
    }

    [HttpPost("students/{userId:guid}/enroll")]
    public async Task<IActionResult> ForceEnroll(
        Guid userId,
        [FromBody] ForceEnrollBody body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ForceEnrollCommand(userId, body.CourseId, body.ExpiresAt, body.Notes), ct);
        return CreatedAtAction(nameof(GetAll), new { userId }, result);
    }

    [HttpPatch("enrollments/{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ActivateEnrollmentCommand(id), ct);
        return NoContent();
    }
}

public record ForceEnrollBody(Guid CourseId, DateTime? ExpiresAt, string? Notes);
