using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public enum AudioLevel
    {
        SFX,
        Music
    }


    [SerializeField]
    private AudioLevel m_level;


    private void Start()
    {
        UpdateVolume();
    }


    public void UpdateVolume()
    {
        foreach (Transform t in transform)
        {
            AudioSource source = t.GetComponent<AudioSource>();
            switch (m_level)
            {
                case AudioLevel.SFX:
                    source.volume = SaveData.Instance.sfxVolume / 100;
                    break;
                case AudioLevel.Music:
                    source.volume = SaveData.Instance.musicVolume / 100;
                    break;
            }
        }
    }
}