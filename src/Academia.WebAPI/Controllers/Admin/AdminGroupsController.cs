using Academia.Application.Groups.Commands.AddGroupMember;
using Academia.Application.Groups.Commands.CreateGroup;
using Academia.Application.Groups.Commands.DeleteGroup;
using Academia.Application.Groups.Commands.EnrollGroup;
using Academia.Application.Groups.Commands.RemoveGroupMember;
using Academia.Application.Groups.Commands.UpdateGroup;
using Academia.Application.Groups.Queries.GetGroupById;
using Academia.Application.Groups.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin/groups")]
[Authorize(Policy = "AdminOnly")]
public class AdminGroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminGroupsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetGroupsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGroupByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateGroupRequest body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateGroupCommand(id, body.Name, body.Description), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteGroupCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid id, [FromBody] MemberRequest body, CancellationToken ct)
    {
        await _mediator.Send(new AddGroupMemberCommand(id, body.UserId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveGroupMemberCommand(id, userId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/enroll")]
    public async Task<IActionResult> Enroll(
        Guid id, [FromBody] EnrollGroupRequest body, CancellationToken ct)
    {
        var count = await _mediator.Send(new EnrollGroupCommand(id, body.CourseId), ct);
        return Ok(new { enrolled = count });
    }
}

public record UpdateGroupRequest(string Name, string? Description);
public record MemberRequest(Guid UserId);
public record EnrollGroupRequest(Guid CourseId);
