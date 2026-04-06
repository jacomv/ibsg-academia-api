using Academia.Application.LearningPaths.Queries.GetLearningPathById;
using Academia.Application.LearningPaths.Queries.GetLearningPaths;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Route("api/learning-paths")]
public class LearningPathsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LearningPathsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLearningPathsQuery(PublicOnly: true), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLearningPathByIdQuery(id), ct);
        return Ok(result);
    }
}
