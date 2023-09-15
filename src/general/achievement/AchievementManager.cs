using System.Collections.Generic;
using Godot;

public static class AchievementManager
{
    private static HashSet<Achievement> completedAchievements;

    private static Dictionary<Achievement, int> achievementProgress;

    private static Queue<Achievement> completedAchievementQueue;

    static AchievementManager()
    {
        achievementProgress = new();
        completedAchievementQueue = new();
        completedAchievements = new();
    }

    /// <summary>
    ///   Increases the progress towards an achievement by a specified amount
    /// </summary>
    /// <returns>The current achievement progress</returns>
    public static int IncreaseAchievementProgress(Achievement achievement, int amount = 1)
    {
        if (completedAchievements.Contains(achievement))
            return achievement.RequiredProgressPoints;

        if (achievementProgress.ContainsKey(achievement))
        {
            achievementProgress[achievement] += amount;
        }
        else
        {
            achievementProgress.Add(achievement, amount);
        }

        int progress = achievementProgress[achievement];

        if (progress >= achievement.RequiredProgressPoints)
        {
            completedAchievementQueue.Enqueue(achievement);
        }

        return progress;
    }

    public static void ResolveCompletedAchievements()
    {
        while (completedAchievementQueue.Count > 0)
        {
            Achievement achievement = completedAchievementQueue.Dequeue();
            achievementProgress.Remove(achievement);

            GD.Print($"Got achievement: {achievement.Name}");
        }
    }
}