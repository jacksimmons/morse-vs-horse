using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Each earl has its own index in the static words list.
/// Each adds a word to the list in Start, and accesses only its own word when hovered/clicked.
/// </summary>
public class EarlBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Manager script for all earls.
    /// </summary>
    private static EarlManager Manager => GameObject.FindWithTag("GameController").GetComponent<EarlManager>();

    /// <summary>
    /// The pony this earl sends.
    /// </summary>
    [SerializeField]
    private PonyBehaviour m_pony;

    /// <summary>
    /// The city this earl guards.
    /// </summary>
    [SerializeField]
    private CityBehaviour m_city;

    /// <summary>
    /// The index of this earl's message in the EarlManager's Words list.
    /// </summary>
    private int m_index;

    /// <summary>
    /// The target MorseString. When the input equals the target, the message is
    /// completed.
    /// </summary>
    public static MorseString CurrentMorseTarget { get; private set; } = null;

    /// <summary>
    /// Has the pony begun moving? If so, earl appears on the UI, and can be hovered on.
    /// </summary>
    private bool m_ponyActive = false;

    private TargetHandler m_targetHandler;


    private void Start()
    {
        m_targetHandler = GameObject.FindWithTag("GameController").GetComponent<TargetHandler>();
    }


    /// <summary>
    /// When hovering, target text is set to this earl's message.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        if (m_ponyActive)
        {
            m_targetHandler.SetTarget(Manager.Messages[m_index], m_pony);
        }
    }


    /// <summary>
    /// When leaving hovering, target text is set to the current earl's message.
    /// This will undo the hovering text change, or do nothing if the earl was clicked on.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        if (m_ponyActive)
        {
            m_targetHandler.SetTarget(MorseCode.MorseStringToEnglishString(CurrentMorseTarget ?? new()), m_pony);
        }
    }


    public void OnClicked()
    {
        if (m_ponyActive)
            CurrentMorseTarget = MorseCode.EnglishStringToMorseString(Manager.Messages[m_index]);
    }


    public void ActivateEarl(Difficulty diff)
    {
        // Display and enable earl button.
        m_ponyActive = true;
        GetComponent<Image>().enabled = true;
        GetComponent<Button>().enabled = true;

        if (m_pony.gameObject.activeSelf)
        {
            Debug.LogWarning("Pony is already activated.");
        }

        m_pony.gameObject.SetActive(true);
        m_pony.ActivatePony(diff, m_city, (int)diff + 1);

        // Assign the earl's word based on provided difficulty.
        List<string> messages = Manager.GetUnusedMessagesOfDifficulty(diff);
        
        // Separate the words file by newlines, then remove all words that have already been selected.
        string message = "";

        if (messages.Count > 0)
        {
            message = messages[Random.Range(0, messages.Count)];
        }
        else
        {
            Debug.LogError("Not enough words for the number of earls placed.");
        }

        m_index = Manager.AddMessage(message);
    }
}