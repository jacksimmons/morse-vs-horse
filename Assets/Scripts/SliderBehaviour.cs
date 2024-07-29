using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SliderBehaviour : MonoBehaviour
{
    private Slider m_slider;

    [SerializeField]
    private TMP_Text m_sliderValueText;


    void Awake()
    {
        m_slider = GetComponentInChildren<Slider>();
        m_slider.onValueChanged.AddListener(SetSliderValue);
    }


    public void SetSliderValue(float value)
    {
        m_sliderValueText.text = value.ToString();
        m_slider.value = value;
    }
}