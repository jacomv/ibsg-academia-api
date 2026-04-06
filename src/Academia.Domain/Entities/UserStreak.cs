using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class UserStreak : BaseEntity
{
    private UserStreak() { }

    public UserStreak(Guid userId)
    {
        UserId = userId;
        CurrentStreak = 0;
        LongestStreak = 0;
    }

    public Guid UserId { get; private set; }
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateTime? LastActivityDate { get; private set; }

    public User User { get; private set; } = default!;

    /// <summary>
    /// Records activity for today. If consecutive week, increments streak.
    /// Called when any lesson is completed.
    /// </summary>
    public void RecordActivity()
    {
        var today = DateTime.UtcNow.Date;

        if (LastActivityDate.HasValue)
        {
            var daysSinceLast = (today - LastActivityDate.Value.Date).TotalDays;

            if (daysSinceLast < 1)
                return; // Already recorded today

            if (daysSinceLast <= 7)
            {
                // Within a week — streak continues
                CurrentStreak++;
            }
            else
            {
                // Streak broken — reset
                CurrentStreak = 1;
            }
        }
        else
        {
            CurrentStreak = 1;
        }

        if (CurrentStreak > LongestStreak)
            LongestStreak = CurrentStreak;

        LastActivityDate = today;
    }
}
