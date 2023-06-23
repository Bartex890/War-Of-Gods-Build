using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static void Save(SavedGame save, string gameName)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string saveFolderPath = Application.persistentDataPath + "/Saves";
        string path = saveFolderPath + "/" + gameName + ".save";

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static SavedGame Load(string gameName)
    {
        string path = Application.persistentDataPath + "/Saves/" + gameName + ".save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavedGame data = formatter.Deserialize(stream) as SavedGame;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("File " + path + " doesn't exist! Returning null.");
            //SavedGame data = new SavedGame();
            //return data;
            return null;
        }
    }
}
