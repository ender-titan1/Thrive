using System;
using System.Collections.Generic;

public class PredatorAchievementProgressTracker : IAchievementProgressTracker
{
    private HashSet<uint> killedSpecies = new();

    public int Progress { get; set; }

    public Achievement Achievement { get; set; } = null!;

    public void UpdateProgress(EventArgs unlockArgs, int amount)
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

    public bool IsComplete()
    {
        return Progress >= Achievement.RequiredProgressPoints;
    }
}

public class PredatorUnlockArgs : EventArgs
{
    public PredatorUnlockArgs(Species species)
    {
        Species = species;
    }

    public Species? Species { get; set; }
}