using Academia.Application.Common.Interfaces;
using Academia.Application.Courses.Commands.ArchiveCourse;
using Academia.Application.Courses.Commands.CreateCourse;
using Academia.Application.Courses.Commands.UpdateCourse;
using Academia.Application.Courses.Queries.GetCourseById;
using Academia.Application.Courses.Queries.GetCourses;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Policy = "AdminOnly")]
public class AdminCoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _storage;

    public AdminCoursesController(IMediator mediator, IFileStorageService storage)
    {
        _mediator = mediator;
        _storage = storage;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] CourseStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCoursesQuery(page, pageSize, Status: status, Search: search), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateCourseCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ArchiveCourseCommand(id), ct);
        return NoContent();
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) return BadRequest("File is empty.");

        var url = await _storage.SaveAsync(
            file.OpenReadStream(), file.FileName, file.ContentType, ct);

        return Ok(new { url });
    }
}
