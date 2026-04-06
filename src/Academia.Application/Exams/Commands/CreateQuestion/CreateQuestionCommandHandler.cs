using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Commands.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateQuestionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var examExists = await _context.Exams.AnyAsync(e => e.Id == request.ExamId, cancellationToken);
        if (!examExists) throw new NotFoundException("Exam", request.ExamId);

        var question = new Question(
            request.ExamId, request.Type, request.Text, request.Options,
            request.CorrectAnswer, request.Points, request.Order);

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return question.Id;
    }
}
