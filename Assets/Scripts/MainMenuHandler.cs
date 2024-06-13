using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private Toggle m_fullscreenToggle;
    [SerializeField]
    private Toggle m_easyModeToggle;

    private void Start()
    {
        m_fullscreenToggle.onValueChanged.AddListener(OnFullscreenTogglePressed);
        m_easyModeToggle.onValueChanged.AddListener(OnEasyModeTogglePressed);

        m_fullscreenToggle.isOn = SaveData.Instance.fullscreen;
        m_easyModeToggle.isOn = SaveData.Instance.easyMode;
    }


    public void OnLevelBtnClicked(int level)
    {
        SceneManager.LoadScene("Game");
    }


    public void OnFullscreenTogglePressed(bool toggle)
    {
        SaveData.Instance.fullscreen = toggle;
        Saving.Save();
        Screen.fullScreen = toggle;
    }


    public void OnEasyModeTogglePressed(bool toggle)
    {
        SaveData.Instance.easyMode = toggle;
        Saving.Save();
    }
}
