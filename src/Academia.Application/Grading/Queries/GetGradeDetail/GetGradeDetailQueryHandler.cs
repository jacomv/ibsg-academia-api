using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Grading.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Grading.Queries.GetGradeDetail;

public class GetGradeDetailQueryHandler : IRequestHandler<GetGradeDetailQuery, GradeDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetGradeDetailQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<GradeDetailDto> Handle(
        GetGradeDetailQuery request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .Include(g => g.User)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Questions.OrderBy(q => q.Order))
            .Include(g => g.Exam)
                .ThenInclude(e => e.Course)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Chapter)
                    .ThenInclude(ch => ch != null ? ch.Course : null)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GradeId, cancellationToken);

        if (grade is null)
            throw new NotFoundException("Grade", request.GradeId);

        // Teachers can only see grades for their own courses
        if (_currentUser.IsTeacher)
        {
            var teacherId = grade.Exam.Course?.TeacherId
                ?? grade.Exam.Chapter?.Course?.TeacherId;
            if (teacherId != _currentUser.Id)
                throw new UnauthorizedException("You can only view grades for your own courses.");
        }

        var answers = await _context.ExamAnswers
            .Where(a =>
                a.UserId == grade.UserId &&
                a.ExamId == grade.ExamId &&
                a.AttemptNumber == grade.AttemptNumber)
            .AsNoTracking()
            .ToDictionaryAsync(a => a.QuestionId, cancellationToken);

        var courseTitle = grade.Exam.Course?.Title
            ?? grade.Exam.Chapter?.Course?.Title ?? "—";

        var answerDtos = grade.Exam.Questions.Select(q =>
        {
            answers.TryGetValue(q.Id, out var answer);
            return new AnswerForGradingDto(
                AnswerId: answer?.Id ?? Guid.Empty,
                QuestionId: q.Id,
                QuestionType: q.Type,
                QuestionText: q.Text,
                MaxPoints: q.Points,
                StudentAnswer: answer?.Answer,
                IsCorrect: answer?.IsCorrect,
                PointsEarned: answer?.PointsEarned ?? 0,
                IsPending: answer?.IsCorrect is null &&
                    q.Type is QuestionType.Essay or QuestionType.ShortAnswer
            );
        }).ToList();

        return new GradeDetailDto(
            GradeId: grade.Id,
            ExamId: grade.ExamId,
            ExamTitle: grade.Exam.Title,
            CourseTitle: courseTitle,
            StudentId: grade.UserId,
            StudentFullName: grade.User.FullName,
            StudentEmail: grade.User.Email,
            AttemptNumber: grade.AttemptNumber,
            TotalScore: grade.TotalScore,
            PassingScore: grade.Exam.PassingScore,
            IsPassed: grade.IsPassed,
            Status: grade.Status,
            TeacherFeedback: grade.TeacherFeedback,
            GradedAt: grade.GradedAt,
            Answers: answerDtos
        );
    }
}
