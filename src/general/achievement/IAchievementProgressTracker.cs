using System;

public interface IAchievementProgressTracker
{
    public abstract int Progress { get; set; }

    public abstract void IncreaseProgress(EventArgs unlockArgs, int amount);
}

public class ProgressTrackerFactory<T> : ProgressTrackerFactory
    where T : IAchievementProgressTracker, new()
{
    public override IAchievementProgressTracker MakeNew()
    {
        return new T();
    }
}

public abstract class ProgressTrackerFactory
{
    public abstract IAchievementProgressTracker MakeNew();
}
