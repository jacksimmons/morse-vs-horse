using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private Toggle m_fullscreenToggle;
    [SerializeField]
    private Toggle m_easyModeToggle;
    [SerializeField]
    private GameObject m_resolutionLRBtn;

    private Resolution[] m_monitorResolutions;


    private void Start()
    {
        m_fullscreenToggle.onValueChanged.AddListener(OnFullscreenTogglePressed);
        m_easyModeToggle.onValueChanged.AddListener(OnEasyModeTogglePressed);

        m_fullscreenToggle.isOn = SaveData.Instance.fullscreen;
        m_easyModeToggle.isOn = SaveData.Instance.easyMode;

        m_monitorResolutions = Screen.resolutions;

        // Apply existing resolution and fullscreen settings
        ApplyVideoSettings();
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


    public void OnResolutionLRBtnPressed(bool right)
    {
        int resIndex = Array.IndexOf(m_monitorResolutions, SaveData.Instance.resolution);
        Debug.Assert(resIndex != -1, "Monitor does not support resolution settings.");
        resIndex = ArrayTools.CircularNextIndex(resIndex, m_monitorResolutions.Length, right);

        SaveData.Instance.resolution = m_monitorResolutions[resIndex];
        ApplyVideoSettings();
        Saving.Save();
    }


    private void ApplyVideoSettings()
    {
        Resolution res = SaveData.Instance.resolution;
        if (Array.IndexOf(m_monitorResolutions, res) == -1)
        {
            Debug.Assert(m_monitorResolutions.Length > 0, "Monitor supports no resolutions!");
            res = m_monitorResolutions[0];
        }

        string resText = $"Resolution: {res.width}x{res.height} @ {Mathf.RoundToInt((float)res.refreshRateRatio.value)}Hz";

        m_resolutionLRBtn.transform.Find("Text").GetComponent<TMP_Text>().text = resText;
        Screen.SetResolution(res.width, res.height, SaveData.Instance.fullscreen);
    }
}
