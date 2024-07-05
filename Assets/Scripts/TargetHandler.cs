using TMPro;
using UnityEngine;


public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_targetText;
    [SerializeField]
    private TMP_Text m_targetHintLabel;
    [SerializeField]
    private TMP_Text m_targetHintText;

    private PonyBehaviour m_targetPony;


    private void Start()
    {
        if (SaveData.Instance.easyMode)
        {
            m_targetHintLabel.enabled = true;
            m_targetHintText.enabled = true;
        }
    }


    /// <summary>
    /// Set the display text of the target morse string.
    /// </summary>
    public void SetTarget(string targetTxt)
    {
        if (SaveData.Instance.easyMode)
        {
            m_targetText.text = targetTxt;
            m_targetHintText.text = $"{MorseCode.EnglishStringToMorseString(targetTxt)}";
        }
        else
        {
            m_targetText.text = targetTxt;
        }
    }


    public void SetPony(PonyBehaviour pony)
    {
        m_targetPony = pony;
    }


    public void CompleteTarget()
    {
        if (m_targetPony == null)
        {
            Debug.LogError("Target pony was null!");
        }

        m_targetPony.Explode();
        m_targetPony = null;
        SetTarget("");
        GetComponent<EarlManager>().CurrentMorseTarget = null;
    }
}