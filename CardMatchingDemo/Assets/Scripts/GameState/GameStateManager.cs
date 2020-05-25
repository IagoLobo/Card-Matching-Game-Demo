using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public GameState LoadGameStateFile()
    {
        string savePath = Application.persistentDataPath + "/GameState.save";
        if(File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Open);

            GameState data = formatter.Deserialize(stream) as GameState;
            stream.Close();
            
            return data;
        }
        else
        {
            Debug.LogError("GameState file not found in " + savePath);
            return null;
        }
    }

    public void SaveGameStateFile(string pn, int numTries, float te, List<int> secretSequence, List<int> idsRevealed)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string savePath = Application.persistentDataPath + "/GameState.save";
        FileStream stream = new FileStream(savePath, FileMode.Create);

        GameState data = new GameState(pn, numTries, te, secretSequence, idsRevealed);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void DeleteGameStateFile()
    {
        
    }
}
