using Academia.Domain.Enums;

namespace Academia.Application.Exams.Dtos;

// Exam for student — NO correct answers exposed
public record ExamDto(
    Guid Id,
    string Title,
    decimal PassingScore,
    int MaxAttempts,
    int? TimeLimitMinutes,
    int Order,
    List<QuestionStudentDto> Questions,
    int AttemptsUsed,
    bool CanAttempt
);

public record QuestionStudentDto(
    Guid Id,
    QuestionType Type,
    string Text,
    List<string> Options,
    decimal Points,
    int Order
);

// Submit input
public record AnswerInput(Guid QuestionId, string? Answer);

// Exam result (after submitting — correct answers revealed for auto-graded)
public record ExamResultDto(
    Guid GradeId,
    Guid ExamId,
    string ExamTitle,
    int AttemptNumber,
    decimal TotalScore,
    decimal PassingScore,
    bool IsPassed,
    GradingStatus Status,
    string? TeacherFeedback,
    DateTime CreatedAt,
    List<AnswerResultDto> Answers
);

public record AnswerResultDto(
    Guid QuestionId,
    string Text,
    QuestionType Type,
    string? StudentAnswer,
    string? CorrectAnswer,   // Only revealed for auto-gradable types
    bool? IsCorrect,
    decimal PointsEarned,
    decimal MaxPoints,
    bool IsPending
);

// Attempt summary for history list
public record AttemptSummaryDto(
    Guid GradeId,
    int AttemptNumber,
    decimal TotalScore,
    bool IsPassed,
    GradingStatus Status,
    DateTime CreatedAt
);
