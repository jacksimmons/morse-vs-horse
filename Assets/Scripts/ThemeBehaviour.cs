using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeBehaviour : MonoBehaviour
{
    private AudioSource m_audio;


    private void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_audio.loop = true;
        m_audio.Play();
    }
}
