using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Course> Courses { get; }
    DbSet<Chapter> Chapters { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<LearningPath> LearningPaths { get; }
    DbSet<LearningPathCourse> LearningPathCourses { get; }
    DbSet<Exam> Exams { get; }
    DbSet<Question> Questions { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<UserProgress> UserProgress { get; }
    DbSet<ExamAnswer> ExamAnswers { get; }
    DbSet<Grade> Grades { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Group> Groups { get; }
    DbSet<GroupMember> GroupMembers { get; }
    DbSet<PointTransaction> PointTransactions { get; }
    DbSet<UserStreak> UserStreaks { get; }
    DbSet<CourseVersion> CourseVersions { get; }
    DbSet<LessonVersion> LessonVersions { get; }
    DbSet<EditorialReview> EditorialReviews { get; }
    DbSet<ReorderAudit> ReorderAudits { get; }
    DbSet<StudentNote> StudentNotes { get; }
    DbSet<Bookmark> Bookmarks { get; }
    DbSet<LessonAttachment> LessonAttachments { get; }
    DbSet<CoursePrerequisite> CoursePrerequisites { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
