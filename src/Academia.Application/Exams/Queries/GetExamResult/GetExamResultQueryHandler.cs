using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Exams.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Queries.GetExamResult;

public class GetExamResultQueryHandler : IRequestHandler<GetExamResultQuery, ExamResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetExamResultQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ExamResultDto> Handle(
        GetExamResultQuery request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .Include(g => g.Exam)
                .ThenInclude(e => e.Questions.OrderBy(q => q.Order))
            .FirstOrDefaultAsync(g =>
                g.UserId == _currentUser.Id &&
                g.ExamId == request.ExamId &&
                g.AttemptNumber == request.AttemptNumber, cancellationToken);

        if (grade is null)
            throw new NotFoundException($"No result found for exam attempt {request.AttemptNumber}.");

        // Load all answers for this attempt
        var answers = await _context.ExamAnswers
            .Where(a =>
                a.UserId == _currentUser.Id &&
                a.ExamId == request.ExamId &&
                a.AttemptNumber == request.AttemptNumber)
            .AsNoTracking()
            .ToDictionaryAsync(a => a.QuestionId, cancellationToken);

        var answerDtos = grade.Exam.Questions.Select(q =>
        {
            answers.TryGetValue(q.Id, out var answer);
            var revealAnswer = q.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse;
            var isPending = answer?.IsCorrect is null;

            return new AnswerResultDto(
                QuestionId: q.Id,
                Text: q.Text,
                Type: q.Type,
                StudentAnswer: answer?.Answer,
                CorrectAnswer: revealAnswer ? q.CorrectAnswer : null,
                IsCorrect: answer?.IsCorrect,
                PointsEarned: answer?.PointsEarned ?? 0,
                MaxPoints: q.Points,
                IsPending: isPending
            );
        }).ToList();

        return new ExamResultDto(
            GradeId: grade.Id,
            ExamId: grade.ExamId,
            ExamTitle: grade.Exam.Title,
            AttemptNumber: grade.AttemptNumber,
            TotalScore: grade.TotalScore,
            PassingScore: grade.Exam.PassingScore,
            IsPassed: grade.IsPassed,
            Status: grade.Status,
            TeacherFeedback: grade.TeacherFeedback,
            CreatedAt: grade.CreatedAt,
            Answers: answerDtos
        );
    }
}
