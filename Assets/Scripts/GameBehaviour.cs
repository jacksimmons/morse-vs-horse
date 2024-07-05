using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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


public struct PonySpawn
{
    public int Start { get; set; }
    public Difficulty Diff { get; set; }
}


public class GameBehaviour : MonoBehaviour
{
    private static PonySpawn[][] m_levelSpawns = new PonySpawn[][]
    {
        // Level 1
        new PonySpawn[] {
            new() { Start = 1, Diff = Difficulty.Easy },
            new() { Start = 12, Diff = Difficulty.Easy },
            new() { Start = 24, Diff = Difficulty.Easy },
            new() { Start = 36, Diff = Difficulty.Easy },
            new() { Start = 48, Diff = Difficulty.Easy }
        }
    };

    /// <summary>
    /// The current level being played.
    /// </summary>
    private int m_level;

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
    private GameObject m_victoryPanel;

    [SerializeField]
    private GameObject m_cities;
    private List<EarlBehaviour> m_inactiveEarls = new();
    private List<EarlBehaviour> m_activeEarls = new();

    [SerializeField]
    private AudioSource m_ponySfx;

    private bool m_checkNoMessagesLeft = false;


    private void Start()
    {
        m_livesImage.sprite = m_livesLostSprites[0];

        foreach (Transform city in m_cities.transform)
        {
            m_inactiveEarls.Add(city.GetComponentInChildren<EarlBehaviour>());
        }

        // Easy takes ~20s so spawn every 15s
        SpawnLevel();

        // Activate victory check after all ponies have spawned
        StartCoroutine(Wait.WaitThen(40, () => m_checkNoMessagesLeft = true));
    }


    private void Update()
    {
        // Handle moving active earls to inactive so they can be reused.
        List<EarlBehaviour> activeEarlsTemp = new(m_activeEarls);
        foreach (EarlBehaviour earl in activeEarlsTemp)
        {
            if (!earl.Pony.PonyActive && !earl.EarlActive)
            {
                m_activeEarls.Remove(earl);
                m_inactiveEarls.Add(earl);
            }
        }

        if (m_checkNoMessagesLeft)
        {
            if (m_activeEarls.Count == 0)
            {
                HandleVictory();
            }
        }
    }


    private void SpawnLevel()
    {
        if (m_level >= m_levelSpawns.Length)
        {
            Debug.LogError("No spawn info for this level.");
            return;
        }

        for (int i = 0; i < m_levelSpawns[m_level].Length; i++)
        {
            PonySpawn ps = m_levelSpawns[m_level][i];
            QueuePonySpawn(ps.Start, ps.Diff);
        }
    }


    /// <summary>
    /// Queues a random pony spawn in the future (in `seconds` seconds).
    /// </summary>
    private void QueuePonySpawn(float seconds, Difficulty diff)
    {
        StartCoroutine(Wait.WaitThen(seconds, () =>
        {
            // Handle the error checking inside the coroutine, in case of race conditions.
            if (m_inactiveEarls.Count == 0)
            {
                Debug.LogError("No earls remaining!");
                return;
            }

            // Choose a random inactive city to send a message.
            EarlBehaviour earl = m_inactiveEarls[Random.Range(0, m_inactiveEarls.Count)];

            // Activate the pony and earl UI.
            earl.ActivateEarl(diff);

            // Move the earl to active earls.
            m_inactiveEarls.Remove(earl);
            m_activeEarls.Add(earl);
            m_ponySfx.Play();
        }));
    }


    public void LoseLife()
    {
        m_livesLost++;

        GetComponent<TargetHandler>().SetTarget("");
        GetComponent<TargetHandler>().SetPony(null);

        // Go to next life lost sprite.
        if (m_livesLost < m_livesLostSprites.Length)
            m_livesImage.sprite = m_livesLostSprites[m_livesLost];

        // All lives lost sprites seen, and another life lost => death.
        else
            m_gameOverPanel.SetActive(true);
    }


    private void HandleVictory()
    {
        m_victoryPanel.SetActive(true);
    }
}
