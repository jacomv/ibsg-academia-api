using Academia.Application.Users.Commands.ChangeUserRole;
using Academia.Application.Users.Commands.CreateUser;
using Academia.Application.Users.Commands.UpdateUser;
using Academia.Application.Users.Queries.GetUserById;
using Academia.Application.Users.Queries.GetUsers;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "AdminOnly")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] UserRole? role = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetUsersQuery(page, pageSize, role, search, isActive), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateUserBody body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateUserCommand(id, body.FirstName, body.LastName, body.Avatar), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid id, [FromBody] ChangeRoleBody body, CancellationToken ct)
    {
        await _mediator.Send(new ChangeUserRoleCommand(id, body.NewRole), ct);
        return NoContent();
    }
}

public record UpdateUserBody(string FirstName, string LastName, string? Avatar);
public record ChangeRoleBody(UserRole NewRole);
