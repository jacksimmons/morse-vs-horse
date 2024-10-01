using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Controls the rate of pony spawns & their difficulties.
/// FNAF style: changable throughout the level to provide dynamic WordDifficulty.
/// </summary>
//public enum GameDiff
//{
//    Easy,
//    Medium,
//    Hard
//}


public enum WordDifficulty
{
    Easy,
    QuiteEasy,
    Intermediate,
    Hard,
    Boss,
    Supercalifragilisticexpialidocious
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
    public int Wait { get; set; }
    public WordDifficulty Diff { get; set; }
    public PonyType Type { get; set; }
}


public class GameBehaviour : MonoBehaviour
{
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

    private int m_poniesGone = 0;

    [SerializeField]
    private GameObject m_gameOverPanel;
    [SerializeField]
    private GameObject m_victoryPanel;
    [SerializeField]
    private TMP_Text m_levelText;

    [SerializeField]
    private GameObject m_cities;
    private List<CityMessageBehaviour> m_inactiveEarls = new();
    private List<CityMessageBehaviour> m_activeEarls = new();

    [SerializeField]
    private AudioSource m_ponySfx;


    private void Start()
    {
        m_livesImage.sprite = m_livesLostSprites[0];

        foreach (Transform city in m_cities.transform)
        {
            m_inactiveEarls.Add(city.GetComponentInChildren<CityMessageBehaviour>());
        }

        SpawnLevel();
    }


    private void Update()
    {
        // Handle moving active earls to inactive so they can be reused.
        List<CityMessageBehaviour> activeEarlsTemp = new(m_activeEarls);
        foreach (CityMessageBehaviour earl in activeEarlsTemp)
        {
            if (!earl.Pony.PonyActive && !earl.Active)
            {
                m_activeEarls.Remove(earl);
                m_inactiveEarls.Add(earl);
                m_poniesGone++;

                if (m_poniesGone == Levels.AllLevels[Levels.SelectedLevel].Spawns.Length)
                {
                    HandleVictory();
                }
            }
        }

        // ! Debug cheats
        if (Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.G))
        {
            HandleVictory();
        }
    }


    private void SpawnLevel()
    {
        int level = SaveData.Instance.levelSelected;

        Debug.Assert(level < Levels.AllLevels.Length, $"No level info for level {level}");

        // Add level info UI
        m_levelText.text = $"Level {level + 1}";
        foreach (Level.Mod mod in Levels.AllLevels[level].Mods)
        {
            m_levelText.text += $"\n{mod.Name}";
        }

        int start = 0;
        for (int i = 0; i < Levels.AllLevels[SaveData.Instance.levelSelected].Spawns.Length; i++)
        {
            PonySpawn ps = Levels.AllLevels[SaveData.Instance.levelSelected].Spawns[i];
            start += ps.Wait;

            QueuePonySpawn(start, ps.Diff, ps.Type);
        }
    }


    /// <summary>
    /// Queues a random pony spawn in the future (in `seconds` seconds).
    /// </summary>
    private void QueuePonySpawn(float seconds, WordDifficulty diff, PonyType type)
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
            CityMessageBehaviour earl = m_inactiveEarls[Random.Range(0, m_inactiveEarls.Count)];

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


    private void HandleVictory()
    {
        m_victoryPanel.SetActive(true);

        // Update highest level beaten, if it has increased.
        // Also set any relevant achievements.
        if (SaveData.Instance.highestLevelBeaten < Levels.SelectedLevel)
        {
            SaveData.Instance.highestLevelBeaten = Levels.SelectedLevel;
            Saving.Save();

            if (SaveData.Instance.highestLevelBeaten >= 8)
            {
                Achievement.GiveAchievement("BEAT_LV_IX");
            }
        }
    }
}
