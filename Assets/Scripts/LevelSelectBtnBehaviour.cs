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
    [SerializeField]
    private int m_endlessLevel;

    private void Awake()
    {
        // Manual Init (e.g. Endless levels)
        if (m_endlessLevel >= 0)
        {
            Init(SaveData.ENDLESS_FLAG + m_endlessLevel);
        }
    }

    public override bool Init(int btnIndex)
    {
        if (!base.Init(btnIndex)) return false;

        if (SaveData.Instance.highestLevelBeaten >= btnIndex - 1)
        {
            m_locked.gameObject.SetActive(false);
            m_btn.interactable = true;
        }

        m_text.text = $"{Roman.ToRoman(btnIndex + 1)}";
        m_btn.onClick.AddListener(() => OnLevelBtnClicked(btnIndex));

        return true;
    }

    public void OnLevelBtnClicked(int level)
    {
        SaveData.Instance.levelSelected = level;
        SceneManager.LoadScene("Game");
    }
}
