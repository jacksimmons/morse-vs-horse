using System;
using System.IO;
using System.Linq;
using UnityEngine;


[Serializable]
public class Version : IComparable<Version>
{
    public enum Stage
    {
        Alpha,
        Beta
    }

    public Stage devStage;
    public int major;
    public int minor;


    public Version(Stage devStage, int major, int minor)
    {
        this.devStage = devStage;
        this.major = major;
        this.minor = minor;
    }


    public int CompareTo(Version other)
    {
        // First compare dev stage difference
        int devDiff = devStage.CompareTo(other.devStage);
        if (devDiff != 0)
            return devDiff;

        // Then compare major diff
        int majorDiff = major;
        if (majorDiff != 0)
            return majorDiff;

        // Now compare minor diff
        int minorDiff = minor;
        if (minorDiff != 0)
            return minorDiff;

        return 0;
    }
}


[Serializable]
public class SaveData
{
    public enum MorseDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    private static SaveData m_inst;
    public static SaveData Instance
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = Saving.Load();

                // Add a check for save version - support migrating save versions
                if (m_inst.saveVersion.CompareTo(currentVersion) < 0)
                {
                    // Set all values to default
                    SaveData newInst = new();

                    foreach (var f in newInst.GetType().GetFields(System.Reflection.BindingFlags.Public))
                    {
                        // Use reflection for all existing save data, and default for the rest.
                        f.SetValue(newInst, f.GetValue(m_inst));
                    }

                    m_inst = newInst;
                }
            }

            return m_inst;
        }
    }
    public static void Reset() { m_inst = new(); }

    public readonly static Version currentVersion = new(Version.Stage.Alpha, 4, 1);

    /// <summary>
    /// Version of the save. Set during write to file.
    /// </summary>
    public Version saveVersion = null;

    /// <summary>
    /// Whether an endless level was selected last from the menu, or not.
    /// </summary>
    public bool endlessSelected = false;
    /// <summary>
    /// Last selected level from the menu.
    /// </summary>
    public int levelSelected = 0;
    /// <summary>
    /// Index of the highest level beaten by the player.
    /// </summary>
    public int highestLevelBeaten = -1;
    /// <summary>
    /// A list of ranks of completion. 0 is default for each level, when
    /// the user beats a level, the level's index in this array is set
    /// to 1 (1 life left), 2 (2 lives left), etc.
    /// </summary>
    public int[] completionRanks = new int[NUM_LEVELS];
    
    private const int NUM_LEVELS = 9;

    public bool fullscreen = false;

    public MorseDifficulty difficulty = MorseDifficulty.Normal;

    // Get the first valid resolution
    public Resolution resolution = AspectRatio.GetSupportedResolutions()[0];

    /// <summary>
    /// Music volume, from 0 to 100.
    /// </summary>
    public float musicVolume = 50;
    /// <summary>
    /// SFX volume, from 0 to 100.
    /// </summary>
    public float sfxVolume = 50;

    /// <summary>
    /// Dot signal must have an input duration of at least ... seconds.
    /// </summary>
    public const float DOT_SIG_LONGER_THAN = 0.0f;

    /// <summary>
    /// Dash signal must have an input duration of at least ... seconds.
    /// </summary>
    public float dashSigLongerThan = 0.3f;
}


public static class Saving
{
    /// <summary>
    /// Load SaveData from the persistent data path.
    /// </summary>
    /// <returns></returns>
    public static SaveData Load()
    {
        return LoadFromFile<SaveData>("Save.dat");
    }


    /// <summary>
    /// Save SaveData to the persistent data path.
    /// </summary>
    public static void Save()
    {
        SaveData.Instance.saveVersion = SaveData.currentVersion;
        SaveToFile(SaveData.Instance, "Save.dat");
    }


    /// <summary>
    /// Serialises objects and saves them to a given file location.
    /// </summary>
    private static void SaveToFile<T>(T serializable, string filename)
    {
        string dest = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(dest)) File.Create(dest).Close();

        // If the provided object is null, delete the file.
        if (serializable == null)
        {
            File.Delete(dest);
            return;
        }

        string json = JsonUtility.ToJson(serializable, true);
        File.WriteAllText(dest, json);
    }


    /// <summary>
    /// Deserialises a serialised object stored in a file.
    /// </summary>
    private static T LoadFromFile<T>(string filename) where T : new()
    {
        string dest = Application.persistentDataPath + "/" + filename;

        if (File.Exists(dest))
        {
            return DeserialiseJson<T>(File.ReadAllText(dest));
        }
        else
        {
            // Restart the function after creating a new T save
            SaveToFile(new T(), filename);
            Debug.LogWarning("File does NOT exist! Returning empty object");
            return LoadFromFile<T>(filename);
        }
    }


    /// <summary>
    /// Deserialised an object from the Resources folder.
    /// </summary>
    /// <typeparam name="T">The object type to deserialise into.</typeparam>
    /// <param name="filename">The local filename (Resources/{filename}).</param>
    /// <returns>The deserialised object.</returns>
    public static T LoadFromResources<T>(string filename) where T : new()
    {
        TextAsset ta = Resources.Load<TextAsset>(filename);
        if (ta == null)
        {
            Debug.LogError($"No resource {filename} exists!");
        }

        return DeserialiseJson<T>(ta.text);
    }


    public static T DeserialiseJson<T>(string json)
    {
        T val = (T)JsonUtility.FromJson(json, typeof(T));
        return val;
    }
}