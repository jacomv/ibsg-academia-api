namespace Academia.Domain.Enums;

public enum UserRole
{
    Administrator = 1,
    Teacher = 2,
    Student = 3
}

public enum CourseStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

public enum AccessType
{
    Free = 1,
    Paid = 2,
    Membership = 3
}

public enum LessonType
{
    Video = 1,
    Audio = 2,
    Text = 3,
    Pdf = 4,
    Mixed = 5
}

public enum ProgressStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4
}

public enum GradingStatus
{
    Pending = 1,
    Auto = 2,
    Manual = 3
}

public enum EnrollmentStatus
{
    Active = 1,
    Expired = 2,
    Cancelled = 3,
    Pending = 4
}
