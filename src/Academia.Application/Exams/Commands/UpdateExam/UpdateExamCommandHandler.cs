using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Commands.UpdateExam;

public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateExamCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if (exam is null) throw new NotFoundException("Exam", request.Id);

        exam.Update(request.Title, request.PassingScore, request.MaxAttempts,
            request.TimeLimitMinutes, request.Order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
