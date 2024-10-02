using System;
using UnityEngine;

public enum MessengerType
{
    Boss,
    Person,
    Pony,
    Train
}

public enum WordDifficulty
{
    Easy,
    Medium,
    Hard,
    VeryHard,
    Chungus,
}


/// <summary>
/// Difficulty of a spawn. A combination of the speed of messenger,
/// the hardest word in the sentence, and the formation of the sentence
/// make up difficulty.
/// </summary>
public readonly struct SpawnDifficulty
{
    public readonly MessengerType Messenger;
    public readonly WordDifficulty WordDiff;


    public SpawnDifficulty(MessengerType messenger, WordDifficulty wordDiff)
    {
        Messenger = messenger;
        WordDiff = wordDiff;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>The spawn difficulty, or null if it couldn't be increased further.</returns>
    public readonly SpawnDifficulty? GetIncreasedDiff()
    {
        WordDifficulty newWordDiff = WordDiff;
        MessengerType newMessenger = Messenger;

        if (WordDiff < WordDifficulty.Chungus)
            newWordDiff++;
        else if (Messenger < MessengerType.Train)
            newMessenger++;
        else
            return null;

        return new SpawnDifficulty(newMessenger, newWordDiff);
    }
}


public struct MessengerSpawn
{
    public float Wait { get; set; }
    public WordDifficulty Diff { get; set; }
    public MessengerType Type { get; set; }
}