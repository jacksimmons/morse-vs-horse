using TMPro;
using UnityEngine;


public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_targetText;
    public string Target
    {
        set
        {
            if (SaveData.Instance.easyMode)
            {
                m_targetText.text = value;
                m_targetHintText.text = $"{MorseCode.EnglishWordToMorseWord(value)}";
            }
            else
            {
                m_targetText.text = value;
            }
        }

        get
        {
            return m_targetText.text;
        }
    }

    [SerializeField]
    private TMP_Text m_targetHintLabel;
    [SerializeField]
    private TMP_Text m_targetHintText;

    public MessengerBehaviour TargetMessenger { get; set; } = null;


    private void Start()
    {
        if (SaveData.Instance.easyMode)
        {
            m_targetHintLabel.enabled = true;
            m_targetHintText.enabled = true;
        }
    }


    public void CompleteTarget()
    {
        if (TargetMessenger == null)
        {
            Debug.LogError("Target pony was null!");
        }

        TargetMessenger.Explode();
        TargetMessenger = null;
        Target = "";
        GetComponent<CityMessageManager>().CurrentMorseTarget = null;
    }
}