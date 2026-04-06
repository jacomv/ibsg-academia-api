using Academia.Application.LearningPaths.Commands.AddCourseToPath;
using Academia.Application.LearningPaths.Commands.ArchiveLearningPath;
using Academia.Application.LearningPaths.Commands.CreateLearningPath;
using Academia.Application.LearningPaths.Commands.RemoveCourseFromPath;
using Academia.Application.LearningPaths.Commands.ReorderPathCourses;
using Academia.Application.LearningPaths.Commands.UpdateLearningPath;
using Academia.Application.LearningPaths.Queries.GetLearningPathById;
using Academia.Application.LearningPaths.Queries.GetLearningPaths;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin/learning-paths")]
[Authorize(Policy = "AdminOnly")]
public class AdminLearningPathsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminLearningPathsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] CourseStatus? status = null, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetLearningPathsQuery(Status: status), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLearningPathByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLearningPathCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return Created($"/api/admin/learning-paths/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateLearningPathBody body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateLearningPathCommand(
            id, body.Name, body.Description, body.Image,
            body.Status, body.AccessType, body.Price, body.GlobalOrder), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ArchiveLearningPathCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/courses")]
    public async Task<IActionResult> AddCourse(
        Guid id, [FromBody] AddCourseBody body, CancellationToken ct)
    {
        var entryId = await _mediator.Send(
            new AddCourseToPathCommand(id, body.CourseId, body.IsRequired), ct);
        return Created($"/api/admin/learning-paths/{id}", new { entryId });
    }

    [HttpDelete("{id:guid}/courses/{courseId:guid}")]
    public async Task<IActionResult> RemoveCourse(Guid id, Guid courseId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveCourseFromPathCommand(id, courseId), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/courses/reorder")]
    public async Task<IActionResult> Reorder(
        Guid id, [FromBody] List<CourseOrderItem> courses, CancellationToken ct)
    {
        await _mediator.Send(new ReorderPathCoursesCommand(id, courses), ct);
        return NoContent();
    }
}

public record UpdateLearningPathBody(
    string Name, string? Description, string? Image,
    CourseStatus Status, AccessType AccessType, decimal? Price, int GlobalOrder);

public record AddCourseBody(Guid CourseId, bool IsRequired);
