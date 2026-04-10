using Academia.Application.Prerequisites.Commands.AddPrerequisite;
using Academia.Application.Prerequisites.Commands.RemovePrerequisite;
using Academia.Application.Prerequisites.Queries.GetPrerequisites;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Policy = "AdminOnly")]
public class AdminPrerequisitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPrerequisitesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{courseId:guid}/prerequisites")]
    public async Task<IActionResult> Get(Guid courseId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPrerequisitesQuery(courseId), ct);
        return Ok(result);
    }

    [HttpPost("{courseId:guid}/prerequisites")]
    public async Task<IActionResult> Add(
        Guid courseId, [FromBody] AddPrerequisiteRequest request, CancellationToken ct)
    {
        await _mediator.Send(new AddPrerequisiteCommand(courseId, request.PrerequisiteCourseId), ct);
        return NoContent();
    }

    [HttpDelete("{courseId:guid}/prerequisites/{prereqCourseId:guid}")]
    public async Task<IActionResult> Remove(Guid courseId, Guid prereqCourseId, CancellationToken ct)
    {
        await _mediator.Send(new RemovePrerequisiteCommand(courseId, prereqCourseId), ct);
        return NoContent();
    }
}

public record AddPrerequisiteRequest(Guid PrerequisiteCourseId);
