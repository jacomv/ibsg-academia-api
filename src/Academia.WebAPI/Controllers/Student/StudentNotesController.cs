using Academia.Application.Notes.Commands.CreateNote;
using Academia.Application.Notes.Commands.DeleteNote;
using Academia.Application.Notes.Commands.UpdateNote;
using Academia.Application.Notes.Queries.GetLessonNotes;
using Academia.Application.Notes.Queries.GetMyNotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Route("api/student")]
[Authorize]
public class StudentNotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentNotesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("lessons/{lessonId:guid}/notes")]
    public async Task<IActionResult> Create(
        Guid lessonId, [FromBody] CreateNoteRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateNoteCommand(lessonId, request.Content, request.TimestampSeconds), ct);
        return Created($"/api/student/notes/{result.Id}", result);
    }

    [HttpGet("lessons/{lessonId:guid}/notes")]
    public async Task<IActionResult> GetByLesson(Guid lessonId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLessonNotesQuery(lessonId), ct);
        return Ok(result);
    }

    [HttpGet("notes")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyNotesQuery(), ct);
        return Ok(result);
    }

    [HttpPut("notes/{noteId:guid}")]
    public async Task<IActionResult> Update(
        Guid noteId, [FromBody] UpdateNoteRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateNoteCommand(noteId, request.Content, request.TimestampSeconds), ct);
        return Ok(result);
    }

    [HttpDelete("notes/{noteId:guid}")]
    public async Task<IActionResult> Delete(Guid noteId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteNoteCommand(noteId), ct);
        return NoContent();
    }
}

public record CreateNoteRequest(string Content, int? TimestampSeconds);
public record UpdateNoteRequest(string Content, int? TimestampSeconds);
