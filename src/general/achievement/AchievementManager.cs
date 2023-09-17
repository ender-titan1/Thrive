using System;
using System.Collections.Generic;

public static class AchievementManager
{
    /// <summary>
    ///   Contains all completed achievements
    /// </summary>
    private static HashSet<Achievement> completedAchievements;

    /// <summary>
    ///   Achievements that are queued to be shown are stored here
    /// </summary>
    private static Queue<Achievement> completedAchievementQueue;

    /// <summary>
    ///   Contains all achievements currently being tracked
    /// </summary>
    private static Dictionary<Achievement, IAchievementProgressTracker> achievementProgress;

    private static Dictionary<string, ProgressTrackerFactory> trackers;

    static AchievementManager()
    {
        achievementProgress = new();
        completedAchievementQueue = new();
        completedAchievements = new();

        // Create factories for progress trackers
        trackers = new()
        {
            { "base", new ProgressTrackerFactory<BaseAchievementProgressTracker>() },
            { "predator", new ProgressTrackerFactory<PredatorAchievementProgressTracker>() },
        };
    }

    /// <summary>
    ///   Fired whenever a new achievement needs to be shown on screen
    /// </summary>
    public static event EventHandler<Achievement>? OnShowNewAchievementPanel;

    /// <summary>
    ///   Increases the progress towards an achievement by a specified amount
    /// </summary>
    /// <returns>The current achievement progress</returns>
    public static int IncreaseAchievementProgress(
        Achievement achievement, EventArgs achievementUnlockArgs, int amount = 1)
    {
        if (completedAchievements.Contains(achievement))
            return achievement.RequiredProgressPoints;

        if (achievementProgress.TryGetValue(achievement, out var progressTracker))
        {
            progressTracker.UpdateProgress(achievementUnlockArgs, amount);
        }
        else
        {
            achievementProgress.Add(achievement, trackers[achievement.TrackerID].MakeNew(achievement));
        }

        IAchievementProgressTracker tracker = achievementProgress[achievement];

        // Check if achievement is completed.
        // If so, add it to the queue of achievements to show
        if (tracker.IsComplete())
        {
            completedAchievementQueue.Enqueue(achievement);
            completedAchievements.Add(achievement);
            achievementProgress.Remove(achievement);
        }

        return tracker.Progress;
    }

    /// <summary>
    ///  Shows the next achievement queued in <see cref="completedAchievementQueue"/>
    ///  on screen
    /// </summary>
    public static void ResolveNextCompletedAchievement()
    {
        if (completedAchievementQueue.Count <= 0)
            return;

        Achievement achievement = completedAchievementQueue.Dequeue();
        OnShowNewAchievementPanel?.Invoke(null, achievement);
    }
}
