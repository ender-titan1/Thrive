using System;

public interface IAchievementProgressTracker
{
    public abstract int Progress { get; set; }

    public abstract Achievement Achievement { get; set; }

    public abstract void UpdateProgress(EventArgs unlockArgs, int amount);

    public abstract bool IsComplete();
}

public class ProgressTrackerFactory<T> : ProgressTrackerFactory
    where T : IAchievementProgressTracker, new()
{
    public override IAchievementProgressTracker MakeNew(Achievement achievement)
    {
        T tracker = new();
        tracker.Achievement = achievement;
        return tracker;
    }
}

public abstract class ProgressTrackerFactory
{
    public abstract IAchievementProgressTracker MakeNew(Achievement achievement);
}
