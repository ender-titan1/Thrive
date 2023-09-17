using System;

public class BaseAchievementProgressTracker : IAchievementProgressTracker
{
    public int Progress { get; set; }

    public Achievement Achievement { get; set; } = null!;

    public void UpdateProgress(EventArgs unlockArgs, int amount)
    {
        Progress += amount;
    }

    public bool IsComplete()
    {
        return Progress >= Achievement.RequiredProgressPoints;
    }
}