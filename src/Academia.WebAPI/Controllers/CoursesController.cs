using Academia.Application.Courses.Queries.GetCourseById;
using Academia.Application.Courses.Queries.GetCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

/// <summary>Public course catalog — no authentication required.</summary>
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCoursesQuery(page, pageSize, Search: search, PublicOnly: true), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id), ct);
        return Ok(result);
    }
}
