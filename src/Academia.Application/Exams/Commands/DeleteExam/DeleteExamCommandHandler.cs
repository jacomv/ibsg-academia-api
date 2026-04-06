using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Commands.DeleteExam;

public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteExamCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if (exam is null) throw new NotFoundException("Exam", request.Id);

        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
