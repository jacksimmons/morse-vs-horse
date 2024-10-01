using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class representing the message hovering over a city. Handles user input on the message (hovering and click).
/// </summary>
/// <remarks>
/// Each message belongs wholly to its city, and its requirements change once its message has been delivered (by you or the
/// horse).
/// </remarks>
public class CityMessageBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CityMessageManager Manager => GameObject.FindWithTag("GameController").GetComponent<CityMessageManager>();

    public bool Active { get; private set; } = false;

    /// <summary>
    /// The city this message belongs to.
    /// </summary>
    [SerializeField]
    private CityBehaviour m_city;

    /// <summary>
    /// The pony that will transmit this message when it activates.
    /// </summary>
    [SerializeField]
    private PonyBehaviour m_pony;
    public PonyBehaviour Pony => m_pony;

    /// <summary>
    /// The index of the message in the manager's Words list.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The object handling the player's input target.
    /// </summary>
    private TargetHandler m_targetHandler;

    /// <summary>
    /// The image of a scroll which appears upon message activation.
    /// </summary>
    [SerializeField]
    private Image m_scroll;

    /// <summary>
    /// The glow image which appears when hovering.
    /// </summary>
    [SerializeField]
    private Image m_glow;


    private void Start()
    {
        m_targetHandler = GameObject.FindWithTag("GameController").GetComponent<TargetHandler>();
    }


    private void Update()
    {
        // Once the message has been activated check for:

        // If the pony is inactive, and the current target is this message's message, need to make
        // the message disappear.

        // Also the message must disappear when its pony becomes inactive.
        if (Active && !Pony.PonyActive)
        {
            // Message disappear
            if (Manager.CurrentMorseTarget == MorseCode.EnglishStringToMorseString(Manager.ActiveMessages[Index]))
            {
                Manager.CurrentMorseTarget = null;
                m_targetHandler.CompleteTarget();
            }

            // Earl disappear
            DeactivateEarl();
        }
    }


    /// <summary>
    /// When hovering, target text is set to this earl's message.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        if (Pony.PonyActive)
        {
            m_targetHandler.Target = Manager.ActiveMessages[Index];
            Pony.SetPathPulse(true);
        }
    }


    /// <summary>
    /// When leaving hovering, target text is set to the current earl's message.
    /// This will undo the hovering text change, or do nothing if the earl was clicked on.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        if (Pony.PonyActive)
        {
            m_targetHandler.Target = MorseCode.MorseStringToEnglishString(Manager.CurrentMorseTarget ?? new());
            Pony.SetPathPulse(false);
        }
    }


    public void OnClicked()
    {
        if (Pony.PonyActive)
        {
            Manager.CurrentMorseTarget = MorseCode.EnglishStringToMorseString(Manager.ActiveMessages[Index]);
            m_targetHandler.SetPony(Pony);
        }
    }


    public void ActivateEarl(WordDifficulty diff, PonyType type)
    {
        GetComponent<Button>().enabled = true;
        m_glow.enabled = true;
        m_scroll.enabled = true;

        if (Pony.gameObject.activeSelf)
        {
            Debug.LogWarning("Pony is already activated.");
        }

        Pony.gameObject.SetActive(true);
        Pony.ActivatePony(type, m_city, 1);

        // Get all messages belonging to our message's required difficulty.
        List<string> messages = Manager.AllMessages[diff];
        
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

        Index = Manager.AddMessage(message);

        // Set activity flag
        Active = true;
    }


    private void DeactivateEarl()
    {
        GetComponent<Button>().enabled = false;
        m_glow.enabled = false;
        m_scroll.enabled = false;

        // Set activity flag
        Active = false;

        // Disable pulsing
        m_pony.SetPathPulse(false);
    }
}