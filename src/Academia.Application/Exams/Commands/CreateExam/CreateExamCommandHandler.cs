using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;

namespace Academia.Application.Exams.Commands.CreateExam;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateExamCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        var exam = new Exam(
            request.Title, request.CourseId, request.ChapterId,
            request.PassingScore, request.MaxAttempts,
            request.TimeLimitMinutes, request.Order);

        await _context.Exams.AddAsync(exam, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return exam.Id;
    }
}
