using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using Academia.Application.Exams.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Commands.SubmitExam;

public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, ExamResultDto>
{
    private const int TimerGraceSeconds = 60;

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public SubmitExamCommandHandler(
        IApplicationDbContext context, ICurrentUser currentUser, IPublisher publisher)
    {
        _context = context;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<ExamResultDto> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
    {
        // Load exam with questions (ordered)
        var exam = await _context.Exams
            .Include(e => e.Questions.OrderBy(q => q.Order))
            .Include(e => e.Course)
            .Include(e => e.Chapter)
                .ThenInclude(ch => ch != null ? ch.Course : null)
            .FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken);

        if (exam is null)
            throw new NotFoundException("Exam", request.ExamId);

        // Resolve the course this exam belongs to
        var courseId = exam.CourseId ?? exam.Chapter?.CourseId;

        // Verify enrollment (if course exists and is not free)
        if (courseId.HasValue)
        {
            var course = exam.Course ?? exam.Chapter?.Course;
            if (course?.AccessType != AccessType.Free)
            {
                var enrolled = await _context.Enrollments.AnyAsync(e =>
                    e.UserId == _currentUser.Id &&
                    e.CourseId == courseId.Value &&
                    e.Status == EnrollmentStatus.Active, cancellationToken);

                if (!enrolled)
                    throw new UnauthorizedException("You must be enrolled in this course to take this exam.");
            }
        }

        // Timer validation (grace period of 60 seconds)
        if (exam.TimeLimitMinutes.HasValue)
        {
            var elapsed = (DateTime.UtcNow - request.StartedAt).TotalSeconds;
            var allowed = exam.TimeLimitMinutes.Value * 60 + TimerGraceSeconds;

            if (elapsed > allowed)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["timer"] = [$"Time limit exceeded. Allowed: {exam.TimeLimitMinutes}m + {TimerGraceSeconds}s grace."]
                });
        }

        // Attempt count check
        var attemptsMade = await _context.Grades
            .CountAsync(g => g.UserId == _currentUser.Id && g.ExamId == exam.Id, cancellationToken);

        if (!exam.CanAttempt(attemptsMade))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["attempts"] = [$"Maximum attempts ({exam.MaxAttempts}) reached for this exam."]
            });

        var attemptNumber = attemptsMade + 1;

        // Grade the exam using domain logic
        var submissions = request.Answers.Select(a => (a.QuestionId, a.Answer));
        var examResult = exam.Grade(submissions);

        // Persist exam answers + grade in one transaction
        var answers = exam.Questions.Select(q =>
        {
            var submitted = request.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
            var qResult = examResult.QuestionResults.FirstOrDefault(r => r.QuestionId == q.Id);

            return new ExamAnswer(
                userId: _currentUser.Id,
                examId: exam.Id,
                questionId: q.Id,
                answer: submitted?.Answer,
                isCorrect: qResult?.IsPending == true ? null : qResult?.IsCorrect,
                pointsEarned: qResult?.PointsEarned ?? 0,
                attemptNumber: attemptNumber
            );
        }).ToList();

        var grade = new Grade(
            userId: _currentUser.Id,
            examId: exam.Id,
            attemptNumber: attemptNumber,
            totalScore: examResult.TotalScore,
            isPassed: examResult.IsPassed,
            status: examResult.GradingStatus
        );

        await _context.ExamAnswers.AddRangeAsync(answers, cancellationToken);
        await _context.Grades.AddAsync(grade, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event if exam was auto-graded and passed
        if (examResult.IsPassed && examResult.GradingStatus == GradingStatus.Auto)
            await _publisher.Publish(
                new ExamPassedEvent(_currentUser.Id, exam.Id, exam.Title, examResult.TotalScore),
                cancellationToken);

        // Build result DTO — reveal correct answers only for auto-gradable types
        var answerDtos = exam.Questions.Select(q =>
        {
            var answer = answers.First(a => a.QuestionId == q.Id);
            var qResult = examResult.QuestionResults.First(r => r.QuestionId == q.Id);
            var revealAnswer = q.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse;

            return new AnswerResultDto(
                QuestionId: q.Id,
                Text: q.Text,
                Type: q.Type,
                StudentAnswer: answer.Answer,
                CorrectAnswer: revealAnswer ? q.CorrectAnswer : null,
                IsCorrect: answer.IsCorrect,
                PointsEarned: answer.PointsEarned,
                MaxPoints: q.Points,
                IsPending: qResult.IsPending
            );
        }).ToList();

        var examTitle = exam.Title;

        return new ExamResultDto(
            GradeId: grade.Id,
            ExamId: exam.Id,
            ExamTitle: examTitle,
            AttemptNumber: attemptNumber,
            TotalScore: examResult.TotalScore,
            PassingScore: exam.PassingScore,
            IsPassed: examResult.IsPassed,
            Status: examResult.GradingStatus,
            TeacherFeedback: null,
            CreatedAt: grade.CreatedAt,
            Answers: answerDtos
        );
    }
}
