using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public struct Mod
{
    public string Name;
}

public static class Levels
{
    public static int SelectedLevel => SaveData.Instance.levelSelected;
    public static readonly Level[] AllLevels = new Level[]
    {
        // Lv1
        new(
        new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person }
        }),

        // Lv2
        new(
        new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Medium, Type = MessengerType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Medium, Type = MessengerType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = MessengerType.Person }
        }),

        // Lv3
        new(
        new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.VeryHard, Type = MessengerType.Boss },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = MessengerType.Person }
        }, "Message for Sir Lancelot"),

        // Lv4
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 12, Diff = WordDifficulty.Medium, Type = MessengerType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 12, Diff = WordDifficulty.Medium, Type = MessengerType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = MessengerType.Pony }
        }),

        // Lv5
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Hard, Type = MessengerType.Person },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Chungus, Type = MessengerType.Boss }
        }, "Message for the King"),

        // Lv6
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = MessengerType.Person }
        }, "Norman Invasion!"),

        // Lv7
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Hard, Type = MessengerType.Person },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = MessengerType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Medium, Type = MessengerType.Pony }
        }),

        // Lv8
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Hard, Type = MessengerType.Person },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Hard, Type = MessengerType.Person }
        }),

        // Lv9
        new(new MessengerSpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.VeryHard, Type = MessengerType.Boss },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Medium, Type = MessengerType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Hard, Type = MessengerType.Person }
        })
    };
}

public class Level
{
    public ReadOnlyCollection<MessengerSpawn> Spawns { get; }
    protected List<MessengerSpawn> m_spawns;

    public string Name { get; }

    
    public Level(MessengerSpawn[] spawns, string name = "")
    {
        m_spawns = new(spawns);
        Spawns = new(m_spawns);
        Name = name;
    }


    /// <summary>
    /// Starts the level, calling `callback` for each pony spawn
    /// to be queued.
    /// </summary>
    /// <param name="callback"></param>
    public virtual void Start(Action<float, SpawnDifficulty> callback)
    {
        float start = 0;
        foreach (MessengerSpawn spawn in Spawns)
        {
            start += spawn.Wait;
            callback(start, new(spawn.Type, spawn.Diff));
        }
    }
}

public class EndlessLevel : Level
{
    public EndlessLevel(MessengerSpawn[] spawns, string name = "") : base(spawns, "Endless " + name) { }


    /// <summary>
    /// Similar to Start in Level, except it queues a vast number of spawns.
    /// The difficulty of the spawns starts at that of the provided spawns,
    /// and slowly increases to max difficulty.
    /// </summary>
    /// <param name="callback"></param>
    public override void Start(Action<float, SpawnDifficulty> callback)
    {
        float start = 0;
        // Modifiable spawn list that won't affect Level's persistent data
        // @todo Test this doesn't affect persistent data (try the same level
        // the endless is based on, after playing the endless version)
        List<MessengerSpawn> spawns = new(m_spawns);

        for (int i = 0; i < 10000; i++)
        {
            // One whole queueing operation
            for (int j = 0; j < spawns.Count; j++)
            {
                MessengerSpawn spawnGet = spawns[j];
                SpawnDifficulty diff = new(spawnGet.Type, spawnGet.Diff);

                start += spawnGet.Wait;
                callback(start, diff);

                // Increase difficulty of one messenger each loop, the messenger whose difficulty increases
                // also changes sequentially.
                if ((i+1) % (j+1) == 0)
                {
                    var incDiff = diff.GetIncreasedDiff();
                    if (incDiff == null)
                    {
                        return;
                    }

                    diff = incDiff ?? diff;
                    spawns[j] = new() { Wait = spawnGet.Wait, Type = diff.Messenger, Diff = diff.WordDiff };
                }
            }
        }
    }
}