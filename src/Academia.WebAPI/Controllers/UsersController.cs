using Academia.Application.Auth.Commands.ChangePassword;
using Academia.Application.Common.Interfaces;
using Academia.Application.Users.Commands.UpdateOwnProfile;
using Academia.Application.Users.Queries.GetOwnProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Route("api/users/me")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _storage;

    public UsersController(IMediator mediator, IFileStorageService storage)
    {
        _mediator = mediator;
        _storage = storage;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
        => Ok(await _mediator.Send(new GetOwnProfileQuery(), ct));

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateOwnProfileCommand command, CancellationToken ct)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) return BadRequest(new { error = "File is empty." });
        var url = await _storage.SaveAsync(file.OpenReadStream(), file.FileName, file.ContentType, ct);
        return Ok(new { url });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command, CancellationToken ct)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
