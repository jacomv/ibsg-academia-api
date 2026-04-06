using Academia.Application.Comments.Commands.CreateComment;
using Academia.Application.Comments.Commands.DeleteComment;
using Academia.Application.Comments.Commands.ResolveComment;
using Academia.Application.Comments.Commands.UpdateComment;
using Academia.Application.Comments.Queries.GetLessonComments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all top-level comments (with replies) for a lesson.</summary>
    [HttpGet("api/lessons/{lessonId:guid}/comments")]
    public async Task<IActionResult> GetByLesson(Guid lessonId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetLessonCommentsQuery(lessonId), ct));

    /// <summary>Post a new comment or reply.</summary>
    [HttpPost("api/lessons/{lessonId:guid}/comments")]
    public async Task<IActionResult> Create(
        Guid lessonId,
        [FromBody] CreateCommentRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateCommentCommand(lessonId, body.Content, body.ParentCommentId), ct);
        return Created($"/api/comments/{result.Id}", result);
    }

    /// <summary>Update comment content (author only).</summary>
    [HttpPatch("api/comments/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCommentRequest body,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateCommentCommand(id, body.Content), ct);
        return NoContent();
    }

    /// <summary>Delete a comment (author or admin).</summary>
    [HttpDelete("api/comments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCommentCommand(id), ct);
        return NoContent();
    }

    /// <summary>Mark a comment as resolved (teacher/admin only).</summary>
    [HttpPatch("api/comments/{id:guid}/resolve")]
    [Authorize(Policy = "AdminOrTeacher")]
    public async Task<IActionResult> Resolve(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ResolveCommentCommand(id), ct);
        return NoContent();
    }
}

public record CreateCommentRequest(string Content, Guid? ParentCommentId);
public record UpdateCommentRequest(string Content);
