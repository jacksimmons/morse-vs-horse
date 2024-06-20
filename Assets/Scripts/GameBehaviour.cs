using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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


public enum Difficulty
{
    Easy,
    Medium,
    Hard
}


public class GameBehaviour : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    [SerializeField]
    private Image m_livesImage;

    /// <summary>
    /// Array of sprites, 0th represents 0 lives lost, and so on.
    /// Game over occurs when a life is lost and the final sprite is
    /// displayed.
    /// </summary>
    [SerializeField]
    private Sprite[] m_livesLostSprites;
    private int m_livesLost = 0;

    [SerializeField]
    private GameObject m_gameOverPanel;

    [SerializeField]
    private List<EarlBehaviour> m_earls;


    private void Start()
    {
        m_livesImage.sprite = m_livesLostSprites[0];
        
        QueuePonySpawn(1, Difficulty.Hard);
        QueuePonySpawn(2, Difficulty.Hard);
        QueuePonySpawn(3, Difficulty.Hard);
        //QueuePonySpawn(40, Difficulty.Easy);

        //QueuePonySpawn(50, Difficulty.Hard);
        //QueuePonySpawn(70, Difficulty.Hard);
        //QueuePonySpawn(90, Difficulty.Hard);
    }


    public void LoseLife()
    {
        m_livesLost++;

        // Go to next life lost sprite.
        if (m_livesLost < m_livesLostSprites.Length)
            m_livesImage.sprite = m_livesLostSprites[m_livesLost];

        // All lives lost sprites seen, and another life lost => death.
        else
            m_gameOverPanel.SetActive(true);
    }


    /// <summary>
    /// Queues a random pony spawn in the future (in `seconds` seconds).
    /// </summary>
    private void QueuePonySpawn(float seconds, Difficulty diff)
    {
        StartCoroutine(Wait.WaitThen(seconds, () =>
        {
            // Handle the error checking inside the coroutine, in case of race conditions.
            if (m_earls.Count == 0)
            {
                Debug.LogError("No earls remaining!");
                return;
            }

            EarlBehaviour earl = m_earls[Random.Range(0, m_earls.Count)];
            // Activate the pony and earl UI.
            earl.ActivateEarl(diff);
            m_earls.Remove(earl);
        }));
    }
}
