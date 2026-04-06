using Academia.Application.Gamification.Queries.GetActivityTimeline;
using Academia.Application.Gamification.Queries.GetLastLesson;
using Academia.Application.Gamification.Queries.GetLeaderboard;
using Academia.Application.Gamification.Queries.GetMyPoints;
using Academia.Application.Gamification.Queries.GetStreak;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Route("api/gamification")]
[Authorize]
public class GamificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamificationController(IMediator mediator) => _mediator = mediator;

    /// <summary>Returns current user's points, level, and recent transactions.</summary>
    [HttpGet("points")]
    public async Task<IActionResult> GetPoints(CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyPointsQuery(), ct));

    /// <summary>Returns weekly leaderboard (top 10) and current user's rank.</summary>
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard(CancellationToken ct)
        => Ok(await _mediator.Send(new GetLeaderboardQuery(), ct));

    /// <summary>Returns current user's streak data and 8-week history.</summary>
    [HttpGet("streak")]
    public async Task<IActionResult> GetStreak(CancellationToken ct)
        => Ok(await _mediator.Send(new GetStreakQuery(), ct));

    /// <summary>Returns daily activity for the last N days (default 14).</summary>
    [HttpGet("timeline")]
    public async Task<IActionResult> GetTimeline([FromQuery] int days = 14, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetActivityTimelineQuery(days), ct));

    /// <summary>Returns the most recently accessed in-progress lesson, or null.</summary>
    [HttpGet("last-lesson")]
    public async Task<IActionResult> GetLastLesson(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLastLessonQuery(), ct);
        if (result is null) return NoContent();
        return Ok(result);
    }
}
