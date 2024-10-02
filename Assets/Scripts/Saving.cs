using System;
using System.IO;
using System.Linq;
using UnityEngine;


public class Version : IComparable<Version>
{
    public enum Stage
    {
        Alpha,
        Beta
    }

    private Stage m_devStage;
    private int m_major;
    private int m_minor;


    public Version(Stage devStage, int major, int minor)
    {
        m_devStage = devStage;
        m_major = major;
        m_minor = minor;
    }


    public int CompareTo(Version other)
    {
        // First compare dev stage difference
        int devDiff = m_devStage.CompareTo(other.m_devStage);
        if (devDiff != 0)
            return devDiff;

        // Then compare major diff
        int majorDiff = m_major;
        if (majorDiff != 0)
            return majorDiff;

        // Now compare minor diff
        int minorDiff = m_minor;
        if (minorDiff != 0)
            return minorDiff;

        return 0;
    }
}


[Serializable]
public class SaveData
{
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

    public readonly static Version currentVersion = new(Version.Stage.Alpha, 4, 0);
    public readonly Version saveVersion = currentVersion;


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
    public int[] completionRanks = new int[HIGHEST_LEVEL];
    
    public const int HIGHEST_LEVEL = 8;


    public bool fullscreen = false;
    public bool easyMode = false;

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