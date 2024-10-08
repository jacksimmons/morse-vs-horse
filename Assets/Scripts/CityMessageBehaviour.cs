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
    /// The messenger that will transmit this message when it activates.
    /// </summary>
    [SerializeField]
    private MessengerBehaviour m_messenger;
    public MessengerBehaviour Messenger => m_messenger;

    /// <summary>
    /// The index of the message in the manager's Words list.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The object handling the player's input.
    /// </summary>
    private InputHandler m_inputHandler;

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

    /// <summary>
    /// The timer showing how long left the messenger has before it reaches
    /// its destination.
    /// </summary>
    [SerializeField]
    private TMP_Text m_messengerTimer;


    private void Start()
    {
        m_targetHandler = GameObject.FindWithTag("GameController").GetComponent<TargetHandler>();
        m_inputHandler = m_targetHandler.gameObject.GetComponent<InputHandler>();
    }


    private void Update()
    {
        // Once the message has been activated check for:

        // If the pony is inactive, and the current target is this message's message, need to make
        // the message disappear.

        // Also the message must disappear when its pony becomes inactive.
        if (Active && !Messenger.MessengerActive)
        {
            // Message disappear
            if (Manager.CurrentMorseTarget == MorseCode.EnglishPhraseToMorsePhrase(Manager.ActiveMessages[Index]))
            {
                Manager.CurrentMorseTarget = null;
                m_targetHandler.CompleteTarget();
            }

            // Make the message disappear
            DeactivateMessage();
        }
    }


    /// <summary>
    /// When hovering, target text is set to this city's message.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        if (Messenger.MessengerActive)
        {
            m_messengerTimer.enabled = true;

            // Enable pulse, if this messenger is not set active and the user started hovering over it.
            // Otherwise, if the messenger is active, the telegraph line should stay on, rather than pulsing.
            if (m_targetHandler.TargetMessenger != Messenger)
                Messenger.SetTelegraphPulse(TelegraphPulse.Pulsing);
        }
    }


    /// <summary>
    /// When leaving hovering, target text is set to the current city's message.
    /// This will undo the hovering text change, or do nothing if the city was clicked on.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        if (Messenger.MessengerActive)
        {
            m_messengerTimer.enabled = false;

            // Disable pulse, if this messenger is not set active and the user stopped hovering over it.
            // Otherwise, if the messenger is active, the telegraph should stay on until it is deactivated.
            if (m_targetHandler.TargetMessenger != Messenger)
                Messenger.SetTelegraphPulse(TelegraphPulse.Off);

            // Set pulse for the target messenger (if it's set) to be On again, as pulsing of other messengers'
            // lines can cause the target's lines to stay off.
            if (m_targetHandler.TargetMessenger)
                m_targetHandler.TargetMessenger.SetTelegraphPulse(TelegraphPulse.On);
        }
    }


    public void OnClicked()
    {
        if (Messenger.MessengerActive)
        {
            // Reset input (if this is a different city)
            m_inputHandler.OnMessageSelected(this);

            // Update morse target and its messenger
            Manager.CurrentMorseTarget = MorseCode.EnglishPhraseToMorsePhrase(Manager.ActiveMessages[Index]);
            m_targetHandler.TargetMessenger = Messenger;

            // Lock in telegraph lines as red
            Messenger.SetTelegraphPulse(TelegraphPulse.On);

            // Turn off telegraph lines for all other messengers
            // This will not introduce bugs, because this messenger has been clicked on, therefore no others
            // can be clicked on at the same time. We can safely turn pulsing off for all other messengers.
            foreach (MessengerBehaviour messenger in MessengerBehaviour.Manager.Messengers)
            {
                // Don't turn pulsing off for our messenger!
                if (messenger == Messenger) continue;

                // Ignore inactive messengers
                if (messenger == null) continue;

                messenger.SetTelegraphPulse(TelegraphPulse.Off);
            }
        }
    }


    public void ActivateMessage(SpawnDifficulty diff)
    {
        GetComponent<Button>().enabled = true;
        m_glow.enabled = true;
        m_scroll.enabled = true;

        if (Messenger.gameObject.activeSelf)
        {
            Debug.LogWarning("Pony is already activated.");
        }

        Messenger.gameObject.SetActive(true);
        Messenger.ActivateMessenger(diff.Messenger, m_city, 1);

        // Get all messages belonging to our message's required difficulty.
        List<string> messages = Manager.AllMessages[diff.WordDiff];
        
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


    private void DeactivateMessage()
    {
        GetComponent<Button>().enabled = false;
        m_glow.enabled = false;
        m_scroll.enabled = false;

        // Set activity flag
        Active = false;

        // Disable pulsing (if user is still hovering for example)
        m_messenger.SetTelegraphPulse(TelegraphPulse.Off);
    }
}