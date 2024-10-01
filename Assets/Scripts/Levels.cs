using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public static class Levels
{
    public static int SelectedLevel => SaveData.Instance.levelSelected;
    public static readonly Level[] AllLevels = new Level[]
    {
        // Lv1
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person }
        }),

        // Lv2
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = PonyType.Person }
        }),

        // Lv3
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Hard, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 12, Diff = WordDifficulty.Easy, Type = PonyType.Person }
        },

        new Level.Mod[] {
            new() { Name = "Message for Big Boris" }
        }),

        // Lv4
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 12, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 12, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Person },
            new() { Wait = 13, Diff = WordDifficulty.Easy, Type = PonyType.Pony }
        }),

        // Lv5
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Intermediate, Type = PonyType.Person },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony }
        }),

        // Lv6
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person },
            new() { Wait = 1, Diff = WordDifficulty.Easy, Type = PonyType.Person }
        },

        new Level.Mod[] {
            new() { Name = "Norman Invasion!" }
        }),

        // Lv7
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.Intermediate, Type = PonyType.Person },
            new() { Wait = 14, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Wait = 13, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony }
        }),

        // Lv8
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Intermediate, Type = PonyType.Person },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Intermediate, Type = PonyType.Person }
        }),

        // Lv9
        new(
        new PonySpawn[] {
            new() { Wait = 1, Diff = WordDifficulty.Boss, Type = PonyType.Boss },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.QuiteEasy, Type = PonyType.Pony },
            new() { Wait = 14, Diff = WordDifficulty.Intermediate, Type = PonyType.Person }
        })
    };
}

public class Level
{
    public struct Mod
    {
        public string Name;
    }

    public readonly PonySpawn[] Spawns;
    public readonly Mod[] Mods;

    public Level(PonySpawn[] spawns)
    {
        Spawns = spawns;
        Mods = Array.Empty<Mod>();
    }

    public Level(PonySpawn[] spawns, Mod[] mods)
    {
        Spawns = spawns;
        Mods = mods;
    }
}