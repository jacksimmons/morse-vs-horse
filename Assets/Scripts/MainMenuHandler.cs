using Steamworks;
using System;
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
    [SerializeField]
    private Slider m_musicVolume;
    [SerializeField]
    private Slider m_sfxVolume;

    private List<Resolution> m_supportedResolutions;

    private void Start()
    {
        m_fullscreenToggle.onValueChanged.AddListener(OnFullscreenTogglePressed);
        m_easyModeToggle.onValueChanged.AddListener(OnEasyModeTogglePressed);

        m_musicVolume.onValueChanged.AddListener(OnMusicVolumeValueChanged);
        m_musicVolume.GetComponentInParent<SliderBehaviour>().SetSliderValue(SaveData.Instance.musicVolume);
        m_sfxVolume.onValueChanged.AddListener(OnSFXVolumeValueChanged);
        m_sfxVolume.GetComponentInParent<SliderBehaviour>().SetSliderValue(SaveData.Instance.sfxVolume);

        m_fullscreenToggle.isOn = SaveData.Instance.fullscreen;
        m_easyModeToggle.isOn = SaveData.Instance.easyMode;

        // Enforce 16:9 aspect ratio
        m_supportedResolutions = new(AspectRatio.GetSupportedResolutions());
        Debug.Assert(m_supportedResolutions.Count > 0, "No supported resolutions");

        // Apply existing resolution and fullscreen settings
        ApplyVideoSettings();
    }


    private void Update()
    {
        // ! Debug reset
        if (Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.G))
        {
            SaveData.Reset();
            Saving.Save();
            SceneManager.LoadScene("Main");
        }
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
        int resIndex = m_supportedResolutions.IndexOf(SaveData.Instance.resolution);
        Debug.Assert(resIndex != -1, "Previous resolution is unsupported.");

        resIndex = m_supportedResolutions.ToArray().CircularNextIndex(resIndex, right);
        print(resIndex);

        SaveData.Instance.resolution = m_supportedResolutions[resIndex];
        ApplyVideoSettings();
    }


    private void ApplyVideoSettings()
    {
        Resolution res = SaveData.Instance.resolution;
        Debug.Assert(m_supportedResolutions.Contains(res), $"Unsupported resolution: {res}.");

        string resText = $"Resolution: {res.width}x{res.height} @ {Mathf.RoundToInt((float)res.refreshRateRatio.value)}Hz";

        m_resolutionLRBtn.transform.Find("Text").GetComponent<TMP_Text>().text = resText;
        Screen.SetResolution(res.width, res.height, SaveData.Instance.fullscreen);
        Saving.Save();
    }


    public void OnMusicVolumeValueChanged(float volume)
    {
        SaveData.Instance.musicVolume = volume;
        Saving.Save();
    }


    public void OnSFXVolumeValueChanged(float volume)
    {
        SaveData.Instance.sfxVolume = volume;
        Saving.Save();
    }
}
