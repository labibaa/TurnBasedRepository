using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileHandler
{
    public static void SaveToJsonData<T>(List<T> toSaveData, string fileName)
    {
        Debug.Log(GetPath(fileName)); // Logs the file path for debugging purposes
        string content = JsonHelper.ToJson<T>(toSaveData.ToArray()); // Converts the list to JSON format
        WriteFile(GetPath(fileName), content); // Writes the JSON data to a file
    }

    public static List<T> LoadJsonData<T>(string fileName)
    {
        string savedData = ReadFile(GetPath(fileName)); // Reads the file content

        // If the file is empty or contains only an empty object, return an empty list
        if (string.IsNullOrEmpty(savedData) || savedData == "{}")
        {
            return new List<T>();
        }

        // Convert JSON string back to a list of objects and return
        List<T> result = JsonHelper.FromJson<T>(savedData).ToList();
        return result;
    }

    private static string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName; // Returns the full path where the file is stored
    }

    private static void WriteFile(string path, string contentToSave)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create); // Creates a new file or overwrites an existing file
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(contentToSave); // Writes the JSON content to the file
        }
    }

    private static string ReadFile(string filePath)
    {
        if (File.Exists(filePath)) // Checks if the file exists
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string savedData = reader.ReadToEnd(); // Reads all content from the file
                return savedData;
            }
        }
        return ""; // Returns an empty string if the file does not exist
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json); // Converts JSON string into a Wrapper object
            return wrapper.Items; // Returns the list of objects stored inside the wrapper
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>(); // Creates a wrapper object
            wrapper.Items = array; // Stores the array inside the wrapper
            return JsonUtility.ToJson(wrapper); // Converts the wrapper object into a JSON string
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>(); // Creates a wrapper object
            wrapper.Items = array; // Stores the array inside the wrapper
            return JsonUtility.ToJson(wrapper, prettyPrint); // Converts the wrapper object into a formatted JSON string
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items; // Holds an array of objects for serialization and deserialization
        }
    }
}
