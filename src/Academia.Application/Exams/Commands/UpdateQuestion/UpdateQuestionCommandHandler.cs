using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Commands.UpdateQuestion;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        if (question is null) throw new NotFoundException("Question", request.Id);

        question.Update(request.Type, request.Text, request.Options,
            request.CorrectAnswer, request.Points, request.Order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
