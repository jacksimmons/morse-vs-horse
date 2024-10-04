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
//public enum GameDiff
//{
//    Easy,
//    Medium,
//    VeryHard
//}


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
    private List<CityMessageBehaviour> m_inactiveMessages = new();
    private List<CityMessageBehaviour> m_activeMessages = new();

    [SerializeField]
    private AudioSource m_ponySfx;


    private void Start()
    {
        m_livesImage.sprite = m_livesLostSprites[0];

        foreach (Transform city in m_cities.transform)
        {
            m_inactiveMessages.Add(city.GetComponentInChildren<CityMessageBehaviour>());
        }

        SpawnLevel();
    }


    private void Update()
    {
        Level level = Levels.AllLevels[Levels.SelectedLevel];

        // Handle moving active earls to inactive so they can be reused.
        List<CityMessageBehaviour> activeEarlsTemp = new(m_activeMessages);
        foreach (CityMessageBehaviour msg in activeEarlsTemp)
        {
            if (!msg.Messenger.MessengerActive && !msg.Active)
            {
                m_activeMessages.Remove(msg);
                m_inactiveMessages.Add(msg);
                m_poniesGone++;

                // If all messengers have come and gone, and you are still alive, then you have won.
                // If endless mode is selected, obviously this is not the case.
                if (!SaveData.Instance.endlessSelected && m_poniesGone == Levels.AllLevels[Levels.SelectedLevel].Spawns.Count)
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
        // Get level
        int levelIndex = SaveData.Instance.levelSelected;
        Level level = Levels.AllLevels[levelIndex];

        // Add level info UI
        m_levelText.text = SaveData.Instance.endlessSelected
            ? "Endless"
            : $"Level {levelIndex + 1}\n{level.Name}";

        // Make the level endless if necessary
        if (SaveData.Instance.endlessSelected)
        {
            level = new EndlessLevel(level.Spawns.ToArray());
        }

        // Callback which the level executes to activate each spawn
        Action<float, SpawnDifficulty> callback =
        (float secs, SpawnDifficulty diff) =>
        {
            QueueMessengerSpawn(secs, diff);
        };
        level.Start(callback);
    }


    /// <summary>
    /// Queues a messenger spawn in the future, with a generated phrase (in `wait` seconds).
    /// </summary>
    private void QueueMessengerSpawn(float wait, SpawnDifficulty diff)
    {
        StartCoroutine(Wait.WaitThen(wait, () =>
        {
            // Handle the error checking inside the coroutine, in case of race conditions.
            if (m_inactiveMessages.Count == 0)
            {
                Debug.LogError("No earls remaining!");
                return;
            }

            // Choose a random inactive city to send a message.
            CityMessageBehaviour msg = m_inactiveMessages[Random.Range(0, m_inactiveMessages.Count)];

            // Activate the pony and earl UI.
            msg.ActivateMessage(diff);

            // Set the city's message to active.
            m_inactiveMessages.Remove(msg);
            m_activeMessages.Add(msg);
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
        int completionRank = 3 - m_livesLost;

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

        // Update the completion rank (1-star, 2-star, 3-star) if improved upon.
        // Don't improve completion rank if easy mode is on. Beating on easy mode is
        // equivalent to a 0-star completion.
        if (SaveData.Instance.completionRanks[Levels.SelectedLevel] < completionRank && !SaveData.Instance.easyMode)
        {
            SaveData.Instance.completionRanks[Levels.SelectedLevel] = completionRank;
        }
    }
}
