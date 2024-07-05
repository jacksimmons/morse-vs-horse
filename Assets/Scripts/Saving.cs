using System;
using System.IO;
using UnityEngine;


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
            }
            return m_inst;
        }
    }

    public byte highestLevelBeaten = 0;
    public bool fullscreen = false;
    public bool easyMode = false;
    public Resolution resolution = Screen.currentResolution;
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