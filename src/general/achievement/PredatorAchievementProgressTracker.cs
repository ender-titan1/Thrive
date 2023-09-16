using System;
using System.Collections.Generic;

public class PredatorAchievementProgressTracker : IAchievementProgressTracker
{
    private HashSet<uint> killedSpecies;

    public PredatorAchievementProgressTracker()
    {
        killedSpecies = new();
    }

    public int Progress { get; set; }

    public void IncreaseProgress(EventArgs unlockArgs, int amount)
    {
        if (unlockArgs is PredatorUnlockArgs args)
        {
            if (killedSpecies.Contains(args.Species!.ID))
            {
                Progress += amount;
                killedSpecies.Add(args.Species!.ID);
            }
        }
        else
        {
            throw new ArgumentException(
                $"{nameof(PredatorAchievementProgressTracker)} requires {nameof(PredatorUnlockArgs)}");
        }
    }
}

public class PredatorUnlockArgs : EventArgs
{
    public Species? Species { get; set; }
}