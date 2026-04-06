using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Grading.Commands.ManualGrade;

public class ManualGradeCommandHandler : IRequestHandler<ManualGradeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public ManualGradeCommandHandler(
        IApplicationDbContext context, ICurrentUser currentUser, IPublisher publisher)
    {
        _context = context;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task Handle(ManualGradeCommand request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .Include(g => g.Exam)
                .ThenInclude(e => e.Questions)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Course)
            .Include(g => g.Exam)
                .ThenInclude(e => e.Chapter)
                    .ThenInclude(ch => ch != null ? ch.Course : null)
            .FirstOrDefaultAsync(g => g.Id == request.GradeId, cancellationToken);

        if (grade is null)
            throw new NotFoundException("Grade", request.GradeId);

        // Security: teacher can only grade exams from their own courses
        if (_currentUser.IsTeacher)
        {
            var courseTeacherId = grade.Exam.Course?.TeacherId
                ?? grade.Exam.Chapter?.Course?.TeacherId;

            if (courseTeacherId != _currentUser.Id)
                throw new UnauthorizedException("You can only grade exams from your own courses.");
        }

        // Load all answers for this attempt
        var answers = await _context.ExamAnswers
            .Where(a =>
                a.UserId == grade.UserId &&
                a.ExamId == grade.ExamId &&
                a.AttemptNumber == grade.AttemptNumber)
            .ToListAsync(cancellationToken);

        // Apply manual scores
        foreach (var score in request.Scores)
        {
            var answer = answers.FirstOrDefault(a => a.Id == score.AnswerId);
            if (answer is not null)
                answer.ApplyManualScore(score.PointsEarned, score.IsCorrect);
        }

        // Recalculate total score
        var totalPoints = grade.Exam.Questions.Sum(q => q.Points);
        var earnedPoints = answers.Sum(a => a.PointsEarned);
        var newTotalScore = totalPoints > 0
            ? Math.Round(earnedPoints / totalPoints * 100, 2)
            : 0m;

        var isPassed = newTotalScore >= grade.Exam.PassingScore;

        grade.ApplyManualGrade(newTotalScore, isPassed, request.Feedback, _currentUser.Id);

        await _context.SaveChangesAsync(cancellationToken);

        // Notify the student that their exam has been graded
        await _publisher.Publish(
            new ExamGradedManuallyEvent(
                grade.UserId, grade.ExamId, grade.Exam.Title, newTotalScore, isPassed),
            cancellationToken);
    }
}
