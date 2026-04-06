using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Exams.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Queries.GetExamById;

public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, ExamDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetExamByIdQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ExamDto> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams
            .Include(e => e.Questions.OrderBy(q => q.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken);

        if (exam is null)
            throw new NotFoundException("Exam", request.ExamId);

        var attemptsUsed = await _context.Grades
            .CountAsync(g => g.UserId == _currentUser.Id && g.ExamId == exam.Id, cancellationToken);

        return new ExamDto(
            Id: exam.Id,
            Title: exam.Title,
            PassingScore: exam.PassingScore,
            MaxAttempts: exam.MaxAttempts,
            TimeLimitMinutes: exam.TimeLimitMinutes,
            Order: exam.Order,
            Questions: exam.Questions.Select(q => new QuestionStudentDto(
                Id: q.Id,
                Type: q.Type,
                Text: q.Text,
                Options: q.Options,
                Points: q.Points,
                Order: q.Order
            )).ToList(),
            AttemptsUsed: attemptsUsed,
            CanAttempt: exam.CanAttempt(attemptsUsed)
        );
    }
}
