using Steamworks;
using System;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public readonly static AppId_t AppId = new(1999720);
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;

    public static GlobalManager Instance
    {
        get
        {
            return GameObject.Find("GlobalManager").GetComponent<GlobalManager>();
        }
    }


    private void Awake()
    {
        // Avoid duplicate instances by self-destroying
        if (Instance != this) Destroy(gameObject);

        // Otherwise we are the first instance, so continue as normal
        DontDestroyOnLoad(gameObject);

        if (!SteamAPI.Init())
        {
            Debug.LogWarning("SteamAPI: failed to initialise.");
        }

        if (SteamAPI.RestartAppIfNecessary(AppId))
        {
            Debug.LogWarning("SteamAPI: restarting executable.");
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("SteamManager: failed to initialise.");
        }

        if (!SteamUser.BLoggedOn())
        {
            Debug.LogWarning("SteamUser: user is not logged into steam.");
        }

        SteamUserStats.RequestCurrentStats();
    }

    private void Start()
    {
        m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        if (!SteamUserStats.RequestCurrentStats())
        {
            Debug.LogError("Couldn't request user stats.");
        }
    }

    private void OnUserStatsReceived(UserStatsReceived_t userStatsReceived)
    {
        if (userStatsReceived.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Couldn't receive user stats.");
            return;
        }
    }

    private void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }
}