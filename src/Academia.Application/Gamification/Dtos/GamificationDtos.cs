namespace Academia.Application.Gamification.Dtos;

public record MyPointsDto(
    int TotalPoints,
    int Level,
    string LevelName,
    int PointsToNextLevel,
    int WeeklyPoints,
    int MonthlyPoints,
    List<PointTransactionDto> RecentTransactions
);

public record PointTransactionDto(
    int Points,
    string Reason,
    DateTime CreatedAt
);

public record LeaderboardDto(
    int MyRank,
    int MyWeeklyPoints,
    List<LeaderboardEntryDto> TopUsers
);

public record LeaderboardEntryDto(
    int Rank,
    Guid UserId,
    string FullName,
    string? Avatar,
    int WeeklyPoints
);

public record StreakDto(
    int CurrentStreak,
    int LongestStreak,
    DateTime? LastActivityDate,
    bool IsActiveThisWeek,
    List<bool> WeekHistory // last 8 weeks: true = active
);

public record ActivityTimelineDto(
    List<DailyActivityDto> Days
);

public record DailyActivityDto(
    DateTime Date,
    int LessonsCompleted,
    int PointsEarned
);

public record LastLessonDto(
    Guid LessonId,
    string LessonTitle,
    Guid CourseId,
    string CourseTitle,
    string? CourseImage,
    decimal ProgressPercentage,
    string LessonType
);

public static class LevelSystem
{
    private static readonly (int MinPoints, string Name)[] Levels =
    [
        (0, "Semilla"),
        (100, "Brote"),
        (300, "Planta"),
        (600, "Árbol"),
        (1000, "Roble"),
        (2000, "Bosque"),
        (4000, "Montaña"),
        (7000, "Estrella"),
        (10000, "Constelación"),
        (15000, "Galaxia"),
    ];

    public static (int Level, string Name, int PointsToNext) Calculate(int totalPoints)
    {
        for (int i = Levels.Length - 1; i >= 0; i--)
        {
            if (totalPoints >= Levels[i].MinPoints)
            {
                var nextLevel = i < Levels.Length - 1 ? Levels[i + 1].MinPoints : int.MaxValue;
                return (i + 1, Levels[i].Name, nextLevel - totalPoints);
            }
        }
        return (1, Levels[0].Name, Levels[1].MinPoints);
    }
}
