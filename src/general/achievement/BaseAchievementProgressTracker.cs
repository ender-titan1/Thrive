using System;

public class BaseAchievementProgressTracker : IAchievementProgressTracker
{
    public int Progress { get; set; }

    public void IncreaseProgress(EventArgs unlockArgs, int amount)
    {
        Progress += amount;
    }
}