using Academia.Application.Common.Interfaces;
using Academia.Application.Courses.Commands.ApproveCourse;
using Academia.Application.Courses.Commands.ArchiveCourse;
using Academia.Application.Courses.Commands.CreateCourse;
using Academia.Application.Courses.Commands.PublishCourse;
using Academia.Application.Courses.Commands.RejectCourse;
using Academia.Application.Courses.Commands.RollbackCourse;
using Academia.Application.Courses.Commands.ScheduleCourse;
using Academia.Application.Courses.Commands.SubmitForReview;
using Academia.Application.Courses.Commands.UpdateCourse;
using Academia.Application.Courses.Commands.ValidateCourse;
using Academia.Application.Courses.Queries.GetCourseById;
using Academia.Application.Courses.Queries.GetCourses;
using Academia.Application.Courses.Queries.GetCourseVersions;
using Academia.Application.Courses.Queries.GetEditorialReviews;
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

    // --- Editorial Workflow ---

    [HttpPost("{id:guid}/submit-review")]
    public async Task<IActionResult> SubmitForReview(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new SubmitForReviewCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id, [FromBody] ApproveRequest? request, CancellationToken ct)
    {
        await _mediator.Send(new ApproveCourseCommand(id, request?.Comment), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id, [FromBody] RejectRequest request, CancellationToken ct)
    {
        await _mediator.Send(new RejectCourseCommand(id, request.Comment), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new PublishCourseCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/schedule")]
    public async Task<IActionResult> Schedule(
        Guid id, [FromBody] ScheduleRequest request, CancellationToken ct)
    {
        await _mediator.Send(new ScheduleCourseCommand(id, request.PublishAt, request.UnpublishAt), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/validate")]
    public async Task<IActionResult> Validate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ValidateCourseCommand(id), ct);
        return Ok(result);
    }

    // --- Versioning ---

    [HttpGet("{id:guid}/versions")]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCourseVersionsQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/rollback/{versionId:guid}")]
    public async Task<IActionResult> Rollback(Guid id, Guid versionId, CancellationToken ct)
    {
        await _mediator.Send(new RollbackCourseCommand(id, versionId), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEditorialReviewsQuery(id), ct);
        return Ok(result);
    }
}

public record ApproveRequest(string? Comment);
public record RejectRequest(string Comment);
public record ScheduleRequest(DateTime PublishAt, DateTime? UnpublishAt);
