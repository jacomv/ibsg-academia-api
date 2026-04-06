using Academia.Application.Exams.Commands.SubmitExam;
using Academia.Application.Exams.Queries.GetExamById;
using Academia.Application.Exams.Queries.GetMyAttempts;
using Academia.Application.Exams.Queries.GetExamResult;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Route("api/exams")]
[Authorize]
public class ExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExam(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetExamByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/attempts")]
    public async Task<IActionResult> GetMyAttempts(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyAttemptsQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/results/{attemptNumber:int}")]
    public async Task<IActionResult> GetResult(Guid id, int attemptNumber, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetExamResultQuery(id, attemptNumber), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id, [FromBody] SubmitExamBody body, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SubmitExamCommand(id, body.Answers, body.StartedAt), ct);
        return Ok(result);
    }
}

public record SubmitExamBody(
    List<Academia.Application.Exams.Dtos.AnswerInput> Answers,
    DateTime StartedAt
);
