using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_inputText;

    [SerializeField]
    private TargetHandler m_targetHandler;


    private const KeyCode INPUT_KEY = KeyCode.Space;

    /// <summary>
    /// Dash signal must have an input duration of at least ... seconds.
    /// </summary>
    private const float DASH_SIG_LONGER_THAN = 0.5f; // seconds

    /// <summary>
    /// Character break must have no input for at least ... seconds.
    /// </summary>
    private const float CHAR_BREAK_LONGER_THAN = 2f; // seconds

    /// <summary>
    /// Word break must have no input for at least ... seconds.
    /// </summary>
    private const float WORD_BREAK_LONGER_THAN = 4f; // seconds


    /// <summary>
    /// Input from the user corresponding to the current Morse Code character
    /// e.g. .-
    /// </summary>
    private MorseChar m_morseCharInput;

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
        if (Input.GetKeyDown(INPUT_KEY))
        {
            m_morseSfx.Play();
        }

        if (Input.GetKeyUp(INPUT_KEY))
        {
            m_morseSfx.Pause();
        }

        // If input is held...
        if (Input.GetKey(INPUT_KEY))
        {
            m_currentSignalLength += Time.deltaTime;
        }
        // If input is not held... (Handle key release & Breaks)
        else
        {
            // Handle key release (Dot or Dash)
            if (Input.GetKeyUp(INPUT_KEY))
            {
                EMorseSignal sig = EMorseSignal.Dot;
                if (m_currentSignalLength > DASH_SIG_LONGER_THAN)
                    sig = EMorseSignal.Dash;

                m_morseCharInput.AddSig(sig);
                m_timeSinceLastSignal = 0;
                m_currentSignalLength = 0;
            }
            // Handle character breaks (if char input is not empty)
            else if (m_morseCharInput.Sig1 != EMorseSignal.None)
            {
                m_timeSinceLastSignal += Time.deltaTime;
                if (m_timeSinceLastSignal > CHAR_BREAK_LONGER_THAN)
                {
                    m_morseStringInput.AddChar(m_morseCharInput);
                    m_morseCharInput = MorseChar.Empty;
                }
            }
            // Handle word breaks (if char break has already happened, and morse input is not empty)
            else if (m_timeSinceLastSignal > CHAR_BREAK_LONGER_THAN && m_morseStringInput.Items.Count > 0)
            {
                m_timeSinceLastSignal += Time.deltaTime;
                if (m_timeSinceLastSignal > WORD_BREAK_LONGER_THAN)
                {
                    m_morseStringInput.AddChar(MorseChar.WordBreak);
                    m_timeSinceLastSignal = 0;
                }
            }
        }
    }


    private void VisualiseMorseInput()
    {
        // Visualise morse code input (morse string + the current unfinished char)
        MorseString visualiser = new(m_morseStringInput);
        visualiser.AddChar(m_morseCharInput);

        m_inputText.text = visualiser.ToString();
    }


    private void CheckMorseInput()
    {
        if (m_morseStringInput.Equals(EarlBehaviour.CurrentMorseTarget))
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
        SetLookupTableIndex(ArrayTools.CircularNextIndex(m_lookupTablePageIndex, m_lookupTablePages.Length, true));
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
