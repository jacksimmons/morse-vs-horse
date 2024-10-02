using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class CityMessageManager : MonoBehaviour
{
    private readonly List<string> m_activeMessages = new();
    public ReadOnlyCollection<string> ActiveMessages { get; private set; }

    private int m_numActiveMessages = 0;

    /// <summary>
    /// The target MorseString. When the input equals the target, the message is
    /// completed.
    /// </summary>
    public MorseString CurrentMorseTarget { get; set; } = null;

    /// <summary>
    /// WordDifficulty to list of messages of that WordDifficulty.
    /// All messages originally in the files, including already used messages.
    /// </summary>
    private Dictionary<WordDifficulty, List<string>> m_allMessages;
    public ReadOnlyDictionary<WordDifficulty, List<string>> AllMessages;


    private void Awake()
    {
        ActiveMessages = new(m_activeMessages);

        int map = SaveData.Instance.levelSelected / LevelSelectGridBehaviour.LEVELS_PER_MAP;
        m_allMessages = new()
        {
            { WordDifficulty.Easy, Init_WordFileToList($"Words/Map{map}/Diff0") },
            { WordDifficulty.Medium, Init_WordFileToList($"Words/Map{map}/Diff1") },
            { WordDifficulty.Hard, Init_WordFileToList($"Words/Map{map}/Diff2") },
            { WordDifficulty.VeryHard, Init_WordFileToList($"Words/Map{map}/Diff3") },
            { WordDifficulty.Chungus, Init_WordFileToList($"Words/Map{map}/Diff4") },
        };
        AllMessages = new(m_allMessages);
    }

    /// <summary>
    /// From a text file, extracts each line as a word in a list.
    /// </summary>
    /// <returns>The list of words.</returns>
    private List<string> Init_WordFileToList(string filename)
    {
        TextAsset words = Resources.Load<TextAsset>(filename);
        return new(words.text.Split("\r\n"));
    }

    /// <summary>
    /// Adds the provided message, and returns the total number of messages, including
    /// the added one.
    /// </summary>
    public int AddMessage(string message)
    {
        m_activeMessages.Add(message);
        return m_numActiveMessages++;
    }
}
