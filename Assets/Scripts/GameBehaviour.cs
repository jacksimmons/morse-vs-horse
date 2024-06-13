using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum Level
{
    Level0,
    Level1,
}

/// <summary>
/// Controls the rate of pony spawns & their difficulties.
/// FNAF style: changable throughout the level to provide dynamic difficulty.
/// </summary>
public enum GameDiff
{
    Easy,
    Medium,
    Hard
}


public class GameBehaviour : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    [SerializeField]
    private TMP_Text m_livesText;
    private int m_lives;

    [SerializeField]
    private GameObject m_gameOverPanel;

    [SerializeField]
    private List<EarlBehaviour> m_earls;


    private void Start()
    {
        m_lives = 1;
        UpdateLivesText();

        QueuePonySpawn(5, PonyDiff.Easy);
        QueuePonySpawn(10, PonyDiff.Hard);
    }


    public void LoseLife()
    {
        m_lives--;
        UpdateLivesText();
        if (m_lives == 0)
        {
            m_gameOverPanel.SetActive(true);
        }
    }


    private void UpdateLivesText()
    {
        m_livesText.text = $"Lives: {m_lives}";
    }


    /// <summary>
    /// Queues a random pony spawn in the future (in `seconds` seconds).
    /// </summary>
    private void QueuePonySpawn(float seconds, PonyDiff diff)
    {
        StartCoroutine(Wait.WaitThen(seconds, () =>
        {
            // Handle the error checking inside the coroutine, in case of race conditions.
            if (m_earls.Count == 0)
                Debug.LogError("No earls remaining!");

            EarlBehaviour earl = m_earls[Random.Range(0, m_earls.Count)];
            earl.ActivatePony(diff);
            m_earls.Remove(earl);
        }));
    }
}
