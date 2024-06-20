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
    /// Difficulty to list of messages of that difficulty.
    /// All messages originally in the files, including already used messages.
    /// </summary>
    private Dictionary<Difficulty, List<string>> s_allMessages;


    private void Awake()
    {
        Messages = new(m_messages);
        s_allMessages = new()
        {
            { Difficulty.Easy, WordListFilenameToList("WordsEasy") },
            { Difficulty.Medium, WordListFilenameToList("WordsMedium") },
            { Difficulty.Hard, WordListFilenameToList("WordsHard") }
        };
    }


    /// <summary>
    /// Return a list of messages not used by any earls, and of the provided
    /// difficulty.
    /// </summary>
    public List<string> GetUnusedMessagesOfDifficulty(Difficulty difficulty)
    {
        return new(s_allMessages[difficulty].Except(m_messages));
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
