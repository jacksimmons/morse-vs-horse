using UnityEngine;

// Sanitised 29/9/24

/// <summary>
/// Class which handles an AudioLevel.
/// All AudioSources attached to GameObjects which descend from the GameObject
/// this script is attached to are managed by this class (are on its AudioLevel).
/// </summary>
/// <remarks>
/// Each AudioLevel can have its volume set in settings, and this class updates
/// the volume of all AudioSources on the level when this happens.
/// </remarks>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// @test
    /// A level on which sounds can play. Each level can be controlled separately
    /// by the player in settings.
    /// </summary>
    public enum AudioLevel
    {
        SFX,
        Music
    }

    /// <summary>
    /// The audio level this manager controls.
    /// </summary>
    [SerializeField]
    private AudioLevel m_level;


    private void Start()
    {
        UpdateVolume();
    }

    /// <summary>
    /// Update the volume of all audio sources this class controls.
    /// (All AudioSources attached to descendants of gameObject).
    /// </summary>
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