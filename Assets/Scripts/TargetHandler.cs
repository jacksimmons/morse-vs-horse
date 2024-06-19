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

    private PonyPathing m_targetPony;


    private void Start()
    {
        if (SaveData.Instance.easyMode)
        {
            m_targetHintLabel.gameObject.SetActive(true);
            m_targetHintText.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// Set the display text of the target morse string, and the pony attempting to deliver
    /// the same message.
    /// </summary>
    public void SetTarget(string targetTxt, PonyPathing pony)
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
        m_targetText.text = "";
    }
}