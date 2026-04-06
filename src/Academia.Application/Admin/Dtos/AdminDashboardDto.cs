namespace Academia.Application.Admin.Dtos;

public record AdminDashboardDto(
    int PublishedCourses,
    int DraftCourses,
    int TotalStudents,
    int TotalTeachers,
    int ActiveLearningPaths,
    int PendingGrades,
    int ActiveEnrollments,
    List<RecentEnrollmentDto> RecentEnrollments,
    List<CourseEnrollmentStatsDto> TopCourses
);

public record RecentEnrollmentDto(
    Guid EnrollmentId,
    string StudentName,
    string StudentEmail,
    string CourseTitle,
    string Status,
    DateTime EnrolledAt
);

public record CourseEnrollmentStatsDto(
    Guid CourseId,
    string Title,
    int ActiveEnrollments,
    int CompletedStudents,
    decimal AverageScore
);

public record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string? Avatar,
    bool IsActive,
    DateTime CreatedAt,
    int ActiveEnrollments,
    int CompletedExams,
    decimal AverageScore,
    List<EnrollmentSummaryDto> Enrollments,
    List<GradeSummaryDto> RecentGrades
);

public record EnrollmentSummaryDto(
    Guid CourseId, string CourseTitle, string Status, DateTime EnrolledAt);

public record GradeSummaryDto(
    string ExamTitle, decimal TotalScore, bool IsPassed, DateTime CreatedAt);
