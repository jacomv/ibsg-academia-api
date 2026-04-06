using Academia.Application.Admin.Queries.GetDashboard;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminDashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMigrationService _migrationService;

    public AdminDashboardController(IMediator mediator, IMigrationService migrationService)
    {
        _mediator = mediator;
        _migrationService = migrationService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAdminDashboardQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// One-time migration endpoint. Reads from Directus DB and imports all data.
    /// Protected: Admin only. Should be disabled after cutover.
    /// </summary>
    [HttpPost("migrate")]
    public async Task<IActionResult> Migrate(
        [FromBody] MigrateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DirectusConnectionString))
            return BadRequest(new { error = "DirectusConnectionString is required." });

        var result = await _migrationService.MigrateFromDirectusAsync(
            request.DirectusConnectionString, ct);

        return result.Success ? Ok(result) : StatusCode(500, result);
    }
}

public record MigrateRequest(string DirectusConnectionString);
