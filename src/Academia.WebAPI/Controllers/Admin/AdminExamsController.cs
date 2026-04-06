using Academia.Application.Exams.Commands.CreateExam;
using Academia.Application.Exams.Commands.CreateQuestion;
using Academia.Application.Exams.Commands.DeleteExam;
using Academia.Application.Exams.Commands.DeleteQuestion;
using Academia.Application.Exams.Commands.UpdateExam;
using Academia.Application.Exams.Commands.UpdateQuestion;
using Academia.Application.Exams.Queries.GetExamById;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminExamsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("exams")]
    public async Task<IActionResult> CreateExam(
        [FromBody] CreateExamCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return Created($"/api/admin/exams/{id}", new { id });
    }

    [HttpGet("exams/{id:guid}")]
    public async Task<IActionResult> GetExam(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetExamByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("exams/{id:guid}")]
    public async Task<IActionResult> UpdateExam(
        Guid id, [FromBody] UpdateExamBody body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateExamCommand(
            id, body.Title, body.PassingScore, body.MaxAttempts, body.TimeLimitMinutes, body.Order), ct);
        return NoContent();
    }

    [HttpDelete("exams/{id:guid}")]
    public async Task<IActionResult> DeleteExam(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteExamCommand(id), ct);
        return NoContent();
    }

    [HttpPost("exams/{examId:guid}/questions")]
    public async Task<IActionResult> CreateQuestion(
        Guid examId, [FromBody] QuestionBody body, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateQuestionCommand(
            examId, body.Type, body.Text, body.Options,
            body.CorrectAnswer, body.Points, body.Order), ct);
        return Created($"/api/admin/questions/{id}", new { id });
    }

    [HttpPut("questions/{id:guid}")]
    public async Task<IActionResult> UpdateQuestion(
        Guid id, [FromBody] QuestionBody body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateQuestionCommand(
            id, body.Type, body.Text, body.Options,
            body.CorrectAnswer, body.Points, body.Order), ct);
        return NoContent();
    }

    [HttpDelete("questions/{id:guid}")]
    public async Task<IActionResult> DeleteQuestion(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteQuestionCommand(id), ct);
        return NoContent();
    }
}

public record UpdateExamBody(
    string Title, decimal PassingScore, int MaxAttempts, int? TimeLimitMinutes, int Order);

public record QuestionBody(
    QuestionType Type, string Text, List<string> Options,
    string? CorrectAnswer, decimal Points, int Order);
