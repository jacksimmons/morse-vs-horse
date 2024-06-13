using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


public class EarlManager : MonoBehaviour
{
    private List<string> m_words = new();
    
    public ReadOnlyCollection<string> Words { get; private set; }
    private int m_numEarls = 0;


    private void Awake()
    {
        Words = new(m_words);
    }


    /// <summary>
    /// Return the next available index (used by EarlBehaviour).
    /// Provided in exchange for the earl's message.
    /// </summary>
    public int GetNextIndex(string word)
    {
        m_words.Add(word);
        return m_numEarls++;
    }
}
