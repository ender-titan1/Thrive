using System;
using System.Collections.Generic;

public static class AchievementManager
{
    private static HashSet<Achievement> completedAchievements;

    private static Queue<Achievement> completedAchievementQueue;

    private static Dictionary<Achievement, IAchievementProgressTracker> achievementProgress;

    private static Dictionary<string, ProgressTrackerFactory> trackers;

    static AchievementManager()
    {
        achievementProgress = new();
        completedAchievementQueue = new();
        completedAchievements = new();

        trackers = new()
        {
            { "base", new ProgressTrackerFactory<BaseAchievementProgressTracker>() },
            { "predator", new ProgressTrackerFactory<PredatorAchievementProgressTracker>() },
        };
    }

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

        if (achievementProgress.ContainsKey(achievement))
        {
            achievementProgress[achievement].IncreaseProgress(achievementUnlockArgs, amount);
        }
        else
        {
            achievementProgress.Add(achievement, trackers[achievement.TrackerID].MakeNew());
        }

        int progress = achievementProgress[achievement].Progress;

        // Check if achievement is completed.
        // If so, add it to the queue of achievements to show
        if (progress >= achievement.RequiredProgressPoints)
        {
            completedAchievementQueue.Enqueue(achievement);
            completedAchievements.Add(achievement);
            achievementProgress.Remove(achievement);
        }

        return progress;
    }

    public static void ResolveNextCompletedAchievement()
    {
        if (completedAchievementQueue.Count <= 0)
            return;

        Achievement achievement = completedAchievementQueue.Dequeue();
        OnShowNewAchievementPanel?.Invoke(null, achievement);
    }
}
