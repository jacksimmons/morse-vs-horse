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
        SaveData.Instance.endlessSelected = false;
        SceneManager.LoadScene("Game");
    }
}
