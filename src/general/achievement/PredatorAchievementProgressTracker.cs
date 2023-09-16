using System;
using System.Collections.Generic;

public class PredatorAchievementProgressTracker : IAchievementProgressTracker
{
    public PredatorAchievementProgressTracker()
    {
        KilledSpecies = new();
    }

    public HashSet<uint> KilledSpecies { get; private set; }

    public int Progress { get; set; }

    public void IncreaseProgress(EventArgs unlockArgs, int amount)
    {
        if (unlockArgs is PredatorUnlockArgs args)
        {
            if (KilledSpecies.Contains(args.Species!.ID))
            {
                Progress += amount;
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