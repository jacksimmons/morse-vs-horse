using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_inputText;
    [SerializeField]
    private CityMessageManager m_earlManager;
    [SerializeField]
    private TargetHandler m_targetHandler;

    private const KeyCode SIGNAL_KEY = KeyCode.Space;

    /// <summary>
    /// Input from the user corresponding to the current Morse Code character
    /// e.g. .-
    /// </summary>
    private MorseChar m_morseCharInput = new();
    public MorseChar MorseCharInput => m_morseCharInput;

    /// <summary>
    /// Input from the user joining all MorseChar inputs to make a word/sentence.
    /// e.g. .... . .-.. .-.. ---
    /// </summary>
    private MorseString m_morseStringInput;


    /// <summary>
    /// The time, in seconds, the current inputted signal has been held.
    /// Purpose: Distinguishing between . and - signals.
    /// </summary>
    private float m_currentSignalLength;

    /// <summary>
    /// The time, in seconds, since the last signal was telegraphed.
    /// Purpose: Identifying breaks between characters/words.
    /// </summary>
    private float m_timeSinceLastSignal;


    [SerializeField]
    private GameObject[] m_lookupTablePages;
    private int m_lookupTablePageIndex = 0;

    [SerializeField]
    private AudioSource m_morseSfx;


    // Start is called before the first frame update
    void Start()
    {
        m_morseStringInput = new();
    }


    void Update()
    {
        HandleMorseInput();
        VisualiseMorseInput();
        CheckMorseInput();
        
        HandleOtherInput();
    }


    private void HandleMorseInput()
    {
        if (Input.GetKeyDown(SIGNAL_KEY))
        {
            m_morseSfx.Play();
        }

        if (Input.GetKeyUp(SIGNAL_KEY))
        {
            m_morseSfx.Pause();
        }

        // If there is no target, ignore logic past the sfx.
        if (m_earlManager.CurrentMorseTarget == null)
        {
            return;
        }

        // If input is held...
        if (Input.GetKey(SIGNAL_KEY))
        {
            m_currentSignalLength += Time.deltaTime;
        }
        // If input is not held... (Handle key release & Breaks)
        else
        {
            // Handle key release (Dot or Dash)
            if (Input.GetKeyUp(SIGNAL_KEY))
            {
                // Add the correct signal type
                if (m_currentSignalLength > MorseCode.DASH_SIG_LONGER_THAN)
                {
                    m_morseCharInput.AddSig(EMorseSignal.Dash);
                }
                else if (m_currentSignalLength > MorseCode.DOT_SIG_LONGER_THAN)
                {
                    m_morseCharInput.AddSig(EMorseSignal.Dot);
                }

                // The signal has ended.
                m_timeSinceLastSignal = 0;
                m_currentSignalLength = 0;
            }

            // Handle character breaks (if the current character is valid)
            else if (MorseCode.MorseCharToEnglishChar(m_morseCharInput) != "")
            {
                //m_timeSinceLastSignal += Time.deltaTime;
                
                // The string does not yet have the char appended, so index = count (* 2 to account for char breaks).
                int charIndex = m_morseStringInput.Items.Count;

                // If the current character is correct, allow a character break
                if (m_earlManager.CurrentMorseTarget.Items[charIndex].Equals(m_morseCharInput))
                {
                    m_morseStringInput.AddChar(m_morseCharInput);
                    m_morseCharInput = MorseChar.Empty;
                }
            }
        }
    }


    private void VisualiseMorseInput()
    {
        // Visualise morse code input (morse string + the current unfinished char)
        MorseString visualiser = new(m_morseStringInput);
        MorseChar visualisedChar = m_morseCharInput;

        // Add a visualisation for the current signal
        if (m_currentSignalLength > MorseCode.DASH_SIG_LONGER_THAN)
        {
            visualisedChar.AddSig(EMorseSignal.Dash);
        }
        else if (m_currentSignalLength > MorseCode.DOT_SIG_LONGER_THAN)
        {
            visualisedChar.AddSig(EMorseSignal.Dot);
        }

        visualiser.AddChar(visualisedChar);
        m_inputText.text = visualiser.ToString();
    }


    private void CheckMorseInput()
    {
        // Check if the messages match. Messages can be completed only after
        // waiting for the final character break.
        if (m_morseStringInput.Equals(m_earlManager.CurrentMorseTarget))
        {
            m_morseStringInput = new();
            m_morseCharInput = new();

            GetComponent<TargetHandler>().CompleteTarget();
        }
    }


    private void HandleOtherInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_morseStringInput = new();
            m_morseCharInput = new();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            CycleLookupTableIndex();
        }
    }


    /// <summary>
    /// Cycles to the next lookup table page in a circular fashion. (Page 0 -> Page 1 -> ... -> Page 0).
    /// </summary>
    public void CycleLookupTableIndex()
    {
        SetLookupTableIndex(m_lookupTablePages.CircularNextIndex(m_lookupTablePageIndex, true));
    }


    /// <summary>
    /// Disables the enabled lookup table page GameObject, and
    /// enables the one specified by index.
    /// </summary>
    private void SetLookupTableIndex(int index)
    {
        m_lookupTablePages[m_lookupTablePageIndex].SetActive(false);
        m_lookupTablePages[index].SetActive(true);

        m_lookupTablePageIndex = index;
    }
}
