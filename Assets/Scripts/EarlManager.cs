using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class EarlManager : MonoBehaviour
{
    private List<string> m_messages = new();
    public ReadOnlyCollection<string> Messages { get; private set; }

    private int m_numEarls = 0;

    /// <summary>
    /// The target MorseString. When the input equals the target, the message is
    /// completed.
    /// </summary>
    public MorseString CurrentMorseTarget { get; set; } = null;


    private List<string> WordListFilenameToList(string filename)
    {
        TextAsset words = Resources.Load<TextAsset>(filename);
        return new(words.text.Split("\r\n").Except(Messages));
    }

    /// <summary>
    /// WordDifficulty to list of messages of that WordDifficulty.
    /// All messages originally in the files, including already used messages.
    /// </summary>
    private Dictionary<WordDifficulty, List<string>> s_allMessages;


    private void Awake()
    {
        Messages = new(m_messages);

        int map = GlobalBehaviour.Instance.Level / 3;
        s_allMessages = new()
        {
            { WordDifficulty.Easy, WordListFilenameToList($"Words/Map{map}/Easy") },
            { WordDifficulty.Medium, WordListFilenameToList($"Words/Map{map}/Medium") },
            { WordDifficulty.Hard, WordListFilenameToList($"Words/Map{map}/Hard") },
            { WordDifficulty.Boss, WordListFilenameToList($"Words/Map{map}/Boss") }
        };
    }


    /// <summary>
    /// Return a list of messages not used by any earls, and of the provided
    /// WordDifficulty.
    /// </summary>
    public List<string> GetUnusedMessagesOfWordDifficulty(WordDifficulty WordDifficulty)
    {
        return new(s_allMessages[WordDifficulty].Except(m_messages));
    }


    /// <summary>
    /// Adds the provided message, and returns the total number of earl
    /// messages thus far.
    /// </summary>
    public int AddMessage(string message)
    {
        m_messages.Add(message);
        return m_numEarls++;
    }
}
