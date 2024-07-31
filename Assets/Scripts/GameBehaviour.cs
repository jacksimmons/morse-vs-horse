using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Controls the rate of pony spawns & their difficulties.
/// FNAF style: changable throughout the level to provide dynamic WordDifficulty.
/// </summary>
public enum GameDiff
{
    Easy,
    Medium,
    Hard
}


public enum WordDifficulty
{
    Easy,
    Medium,
    Hard,
    Boss,
    FinalBoss
}


public enum PonyType
{
    Boss,
    Person,
    Pony,
    Train
}


public struct PonySpawn
{
    public int Start { get; set; }
    public WordDifficulty Diff { get; set; }
    public PonyType Type { get; set; }
}


public class GameBehaviour : MonoBehaviour
{
    private readonly PonySpawn[][] m_startingLevelSpawns = new PonySpawn[][]
    {
        // Level 1
        new PonySpawn[]
        {
            new() { Start = 1, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Start = 12, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Start = 24, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Start = 36, Diff = WordDifficulty.Easy, Type = PonyType.Pony },
            new() { Start = 48, Diff = WordDifficulty.Easy, Type = PonyType.Pony }
        },

        // Level 2
        new PonySpawn[]
        {
            new() { Start = 1, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 12, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 24, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 36, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 48, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 60, Diff = WordDifficulty.Hard, Type = PonyType.Pony }
        },

        // Level 3
        new PonySpawn[]
        {
            new() { Start = 1, Diff = WordDifficulty.Hard, Type = PonyType.Pony },
            new() { Start = 12, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 24, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 36, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 48, Diff = WordDifficulty.Medium, Type = PonyType.Pony },
            new() { Start = 60, Diff = WordDifficulty.Hard, Type = PonyType.Pony },
            new() { Start = 90, Diff = WordDifficulty.Boss, Type = PonyType.Person },
        }
    };

    private PonySpawn[][] m_levelSpawns = new PonySpawn[3][];

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
    private TMP_Text m_levelText;

    [SerializeField]
    private GameObject m_cities;
    private List<EarlBehaviour> m_inactiveEarls = new();
    private List<EarlBehaviour> m_activeEarls = new();

    [SerializeField]
    private AudioSource m_ponySfx;

    private bool m_checkNoMessagesLeft = false;

    private int m_level;
    private int m_totalLevel;

    private readonly PonyType[] m_ponyTypes = (PonyType[])Enum.GetValues(typeof(PonyType));
    private readonly WordDifficulty[] m_wordDiffs = (WordDifficulty[])Enum.GetValues(typeof(WordDifficulty));


    private void Start()
    {
        m_livesImage.sprite = m_livesLostSprites[0];

        foreach (Transform city in m_cities.transform)
        {
            m_inactiveEarls.Add(city.GetComponentInChildren<EarlBehaviour>());
        }

        Array.Copy(m_startingLevelSpawns, m_levelSpawns, m_startingLevelSpawns.Length);
        m_level = GlobalBehaviour.Instance.Level;
        m_totalLevel = m_level;
        StartLevel();
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
                WinLevel();
            }
        }

        // ! Debug cheats
        if (Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.G))
        {
            WinLevel();
        }
    }


    private void StartLevel()
    {
        m_checkNoMessagesLeft = false;
        SpawnLevel();

        // Activate victory check after all ponies have spawned
        PonySpawn last = m_levelSpawns[m_level][^1];

        // Check for no messages remaining
        StartCoroutine(Wait.WaitThen(last.Start, () => m_checkNoMessagesLeft = true));
    }


    private void SpawnLevel()
    {
        Debug.Assert(m_level < m_levelSpawns.Length, $"No level info for level {m_totalLevel}");
        m_levelText.text = $"Level {m_totalLevel + 1}";

        for (int i = 0; i < m_levelSpawns[m_level].Length; i++)
        {
            PonySpawn ps = m_levelSpawns[m_level][i];
            QueuePonySpawn(m_level, ps.Start, ps.Diff, ps.Type);
        }
    }


    /// <summary>
    /// Queues a random pony spawn in the future (in `seconds` seconds).
    /// </summary>
    private void QueuePonySpawn(int level, float seconds, WordDifficulty diff, PonyType type)
    {
        StartCoroutine(Wait.WaitThen(seconds, () =>
        {
            if (m_inactiveEarls.Count == 0)
            {
                // ! This can happen when using the level cheat code.
                Debug.LogWarning("No earls remaining!");
                return;
            }

            if (m_level > level)
            {
                // ! This can happen when using the level cheat code.
                Debug.LogWarning("Word skipped!");
                return;
            }

            // Choose a random inactive city to send a message.
            EarlBehaviour earl = m_inactiveEarls[Random.Range(0, m_inactiveEarls.Count)];

            // Activate the pony and earl UI.
            earl.ActivateEarl(diff, type);

            // Move the earl to active earls.
            m_inactiveEarls.Remove(earl);
            m_activeEarls.Add(earl);
            m_ponySfx.Play();
        }));
    }


    public void LoseLife(string wordLost)
    {
        m_livesLost++;

        TargetHandler th = GetComponent<TargetHandler>();
        if (th.Target == wordLost)
        {
            th.Target = "";
            th.SetPony(null);
        }

        // Go to next life lost sprite.
        if (m_livesLost < m_livesLostSprites.Length)
            m_livesImage.sprite = m_livesLostSprites[m_livesLost];

        // All lives lost sprites seen, and another life lost => death.
        else
            m_gameOverPanel.SetActive(true);
    }


    private void WinLevel()
    {
        if (!GlobalBehaviour.Instance.Endless)
        {
            m_victoryPanel.SetActive(true);

            // Update highest level beaten
            if (SaveData.Instance.highestLevelBeaten < m_level)
            {
                SaveData.Instance.highestLevelBeaten = m_level;
                Saving.Save();
            }
        }
        else
        {
            // If in the third level of the map, go back to the first.
            if ((m_level + 1) % 3 == 0)
            {
                m_level -= 2;
            }
            // Otherwise go to the next level.
            else
            {
                m_level++;
            }

            m_totalLevel++;
            StartLevel();
        }
    }
}
