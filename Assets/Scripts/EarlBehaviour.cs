using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Each earl has its own index in the static words list.
/// Each adds a word to the list in Start, and accesses only its own word when hovered/clicked.
/// </summary>
public class EarlBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EarlManager m_manager;
    private TargetHandler m_targetHandler;
    [SerializeField]
    private PonyPathing m_pony;

    /// <summary>
    /// The index of this earl's message in the EarlManager's Words list.
    /// </summary>
    private int m_index;

    /// <summary>
    /// The target MorseString. When the input equals the target, the message is
    /// completed.
    /// </summary>
    public static MorseString CurrentMorseTarget { get; private set; } = null;


    private void Start()
    {
        m_targetHandler = GameObject.FindWithTag("GameController").GetComponent<TargetHandler>();
        m_manager = m_targetHandler.GetComponent<EarlManager>();

        TextAsset wordsFile = Resources.Load<TextAsset>("WordsEasy");

        // Separate the words file by newlines, then remove all words that have already been selected.
        List<string> words = new(wordsFile.text.Split("\r\n").Except(m_manager.Words));
        string word = "";

        if (words.Count > 0)
        {
            word = words[Random.Range(0, words.Count)];
        }
        else
        {
            Debug.LogError("Not enough words for the number of earls placed.");
        }

        m_index = m_manager.GetNextIndex(word);
    }


    /// <summary>
    /// When hovering, target text is set to this earl's message.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        m_targetHandler.SetTarget(m_manager.Words[m_index], m_pony);
    }


    /// <summary>
    /// When leaving hovering, target text is set to the current earl's message.
    /// This will undo the hovering text change, or do nothing if the earl was clicked on.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        m_targetHandler.SetTarget(MorseCode.MorseStringToEnglishString(CurrentMorseTarget ?? new()), m_pony);
    }


    public void OnClicked()
    {
        CurrentMorseTarget = MorseCode.EnglishStringToMorseString(m_manager.Words[m_index]);
    }


    public void ActivatePony(PonyDiff diff)
    {
        if (m_pony.gameObject.activeSelf)
        {
            Debug.LogWarning("Pony is already activated.");
        }

        m_pony.gameObject.SetActive(true);
        m_pony.Diff = diff;
    }
}