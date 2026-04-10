using Academia.Application.Bookmarks.Commands.ToggleBookmark;
using Academia.Application.Bookmarks.Queries.GetMyBookmarks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Student;

[ApiController]
[Route("api/student")]
[Authorize]
public class StudentBookmarksController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentBookmarksController(IMediator mediator) => _mediator = mediator;

    [HttpPost("lessons/{lessonId:guid}/bookmark")]
    public async Task<IActionResult> Toggle(Guid lessonId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ToggleBookmarkCommand(lessonId), ct);
        return Ok(result);
    }

    [HttpGet("bookmarks")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyBookmarksQuery(), ct);
        return Ok(result);
    }
}
