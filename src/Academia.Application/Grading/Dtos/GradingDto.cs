using Academia.Domain.Enums;

namespace Academia.Application.Grading.Dtos;

public record PendingGradeDto(
    Guid GradeId,
    Guid ExamId,
    string ExamTitle,
    string CourseTitle,
    Guid StudentId,
    string StudentFullName,
    string StudentEmail,
    int AttemptNumber,
    decimal TotalScore,
    GradingStatus Status,
    DateTime CreatedAt
);

public record GradeDetailDto(
    Guid GradeId,
    Guid ExamId,
    string ExamTitle,
    string CourseTitle,
    Guid StudentId,
    string StudentFullName,
    string StudentEmail,
    int AttemptNumber,
    decimal TotalScore,
    decimal PassingScore,
    bool IsPassed,
    GradingStatus Status,
    string? TeacherFeedback,
    DateTime? GradedAt,
    List<AnswerForGradingDto> Answers
);

public record AnswerForGradingDto(
    Guid AnswerId,
    Guid QuestionId,
    QuestionType QuestionType,
    string QuestionText,
    decimal MaxPoints,
    string? StudentAnswer,
    bool? IsCorrect,
    decimal PointsEarned,
    bool IsPending
);

public record ManualScoreInput(Guid AnswerId, decimal PointsEarned, bool IsCorrect);
