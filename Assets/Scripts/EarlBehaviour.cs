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

    public bool EarlActive { get; private set; } = false;

    /// <summary>
    /// The pony this earl sends.
    /// </summary>
    [SerializeField]
    private PonyBehaviour m_pony;
    public PonyBehaviour Pony => m_pony;

    /// <summary>
    /// The city this earl guards.
    /// </summary>
    private CityBehaviour m_city;

    [SerializeField]
    private Image m_glow;
    [SerializeField]
    private Image m_scroll;

    /// <summary>
    /// The index of this earl's message in the EarlManager's Words list.
    /// </summary>
    private int m_index;

    private TargetHandler m_targetHandler;


    private void Start()
    {
        m_targetHandler = GameObject.FindWithTag("GameController").GetComponent<TargetHandler>();
        m_city = GetComponentInParent<CityBehaviour>();
    }


    private void Update()
    {
        // Once the earl has been activated check for:

        // If the pony is inactive, and the current target is this earl's message, need to make
        // the message disappear.

        // Also the earl must disappear when his pony becomes inactive.
        if (EarlActive && !Pony.PonyActive)
        {
            // Message disappear
            if (Manager.CurrentMorseTarget == MorseCode.EnglishStringToMorseString(Manager.Messages[m_index]))
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
            m_targetHandler.SetTarget(Manager.Messages[m_index]);
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
            m_targetHandler.SetTarget(MorseCode.MorseStringToEnglishString(Manager.CurrentMorseTarget ?? new()));
            Pony.SetPathPulse(false);
        }
    }


    public void OnClicked()
    {
        if (Pony.PonyActive)
        {
            Manager.CurrentMorseTarget = MorseCode.EnglishStringToMorseString(Manager.Messages[m_index]);
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

        // Assign the earl's word based on provided WordDifficulty.
        List<string> messages = Manager.GetUnusedMessagesOfWordDifficulty(diff);
        
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

        // Set activity flag
        EarlActive = true;
    }


    private void DeactivateEarl()
    {
        GetComponent<Button>().enabled = false;
        m_glow.enabled = false;
        m_scroll.enabled = false;

        // Set activity flag
        EarlActive = false;

        // Disable pulsing
        m_pony.SetPathPulse(false);
    }
}