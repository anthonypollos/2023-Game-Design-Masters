using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string directoryPath = "", string fileName = "SaveData.lasso")
    {
        dataDirPath = directoryPath;
        dataFileName = fileName;
        Debug.Log(Path.Combine(dataDirPath, dataFileName));
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

    }

    public SavedValues Load()
    {
        //Debug.Log("Loading files");
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        SavedValues loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                        //Debug.Log(dataToLoad);
                    }
                }
                loadedData = JsonUtility.FromJson<SavedValues>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(SavedValues savedValues)
    {
        //Debug.Log("Saving files");
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(savedValues, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    //Debug.Log("Saving Complete");
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error occured when to save data to file: " + fullPath + "\n" + e);
        }
    }
}
