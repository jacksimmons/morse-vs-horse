using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectBtnBehaviour : LevelSelectBehaviour
{
    [SerializeField]
    private Button m_btn;
    [SerializeField]
    private TMP_Text m_text;
    [SerializeField]
    private Image m_locked;

    /// <summary>
    /// Completion rank image, which is set to 1, 2 or 3-ponies.
    /// Is disabled if the level has a completion rank of 0.
    /// </summary>
    [SerializeField]
    private Image m_completionRankImage;

    [SerializeField]
    private Sprite[] m_completionRankSprites;


    public override bool Init(int btnIndex)
    {
        if (!base.Init(btnIndex)) return false;

        if (SaveData.Instance.highestLevelBeaten >= btnIndex - 1)
        {
            m_locked.gameObject.SetActive(false);
            m_btn.interactable = true;
        }

        // Set level number
        m_text.text = $"{Roman.ToRoman(btnIndex + 1)}";

        // Set level listener
        m_btn.onClick.AddListener(() => OnLevelBtnClicked(btnIndex));

        // Set level completion rank (if applicable)
        int completionRank = SaveData.Instance.completionRanks[btnIndex];
        if (completionRank > 0)
        {
            m_completionRankImage.enabled = true;
            m_completionRankImage.sprite = m_completionRankSprites[completionRank - 1];
        }

        return true;
    }


    public void OnLevelBtnClicked(int level)
    {
        SaveData.Instance.levelSelected = level;
        SaveData.Instance.endlessSelected = false;
        Saving.Save();
        SceneManager.LoadScene("Game");
    }
}
