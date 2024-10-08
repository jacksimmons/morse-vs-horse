using Steamworks;

public static class Steam
{
    public static void GiveAchievement(string achievementId)
    {
        if (SteamUserStats.GetAchievement(achievementId, out bool achieved))
        {
            // Don't give an achievement if it has already been achieved
            if (achieved == true) return;

            // Give the user the achievement, and update their stats
            SteamUserStats.SetAchievement(achievementId);
            SteamUserStats.StoreStats();
        }
    }
}