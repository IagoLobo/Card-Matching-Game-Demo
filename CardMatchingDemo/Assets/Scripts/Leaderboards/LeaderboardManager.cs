using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public struct Ranker
{
    public int rankerPlace;
    public string rankerName;
    public int rankerFinalScore;
    public float rankerTimeElapsed;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    public List<Ranker> currentLeaderboard;
    private Ranker currentPlayerAttempt;

    void Awake()
    {
        Instance = this;
        currentLeaderboard = LoadLeaderboardFile();
        if(currentLeaderboard == null)  currentLeaderboard = new List<Ranker>();
        OrderLeaderboard();
        SaveLeaderboardFile();
        currentLeaderboard = LoadLeaderboardFile();
        if(currentLeaderboard == null)  currentLeaderboard = new List<Ranker>();
    }

    public List<Ranker> LoadLeaderboardFile()
    {
        List<Ranker> rankers = new List<Ranker>();

        string savePath = Application.persistentDataPath + "/Leaderboard.save";
        if(File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Open);

            Leaderboard data = formatter.Deserialize(stream) as Leaderboard;
            stream.Close();
            
            return data.leaderboardRankerList;
        }
        else
        {
            Debug.LogError("Leaderboard file not found in " + savePath);
            return null;
        }
    }

    public void SaveLeaderboardFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string savePath = Application.persistentDataPath + "/Leaderboard.save";
        FileStream stream = new FileStream(savePath, FileMode.Create);

        Leaderboard data = new Leaderboard(currentLeaderboard);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    // Saves the current won game
    public void SavePlayerAttempt(int finalPlayerScore, float timeElapsed)
    {
        currentPlayerAttempt = new Ranker();
        currentPlayerAttempt.rankerName = PlayerPrefs.GetString("PlayerName");
        currentPlayerAttempt.rankerFinalScore = finalPlayerScore;
        currentPlayerAttempt.rankerTimeElapsed = timeElapsed;
    }

    public void AddToLeaderboard()
    {
        // First, order current Leaderboard just in case
        OrderLeaderboard();

        // Max of 10 rankers on leaderboard, so check if current player should enter
        if(currentLeaderboard?.Count >= 10)
        {
            // If current player is better than the last place, change players
            if(currentPlayerAttempt.rankerFinalScore < currentLeaderboard[9].rankerFinalScore && 
                currentPlayerAttempt.rankerFinalScore > 0)
            {
                Ranker removedPlayer = currentLeaderboard[9];
                currentLeaderboard.Remove(removedPlayer);
                currentLeaderboard.Add(currentPlayerAttempt);

                // Order once again after the new player is added
                OrderLeaderboard();
            }
        }
        else
        {
            // Add player to leaderboard
            if(currentPlayerAttempt.rankerFinalScore > 0)   currentLeaderboard.Add(currentPlayerAttempt);

            // Order once again after the new player is added
            OrderLeaderboard();
        }

        SaveLeaderboardFile();
    }

    public void OrderLeaderboard()
    {
        if(currentLeaderboard != null)
        {
            // First, order by final score and then by ranker time elapsed
            currentLeaderboard = currentLeaderboard.OrderBy(x => x.rankerFinalScore).ThenBy(x => x.rankerTimeElapsed).ToList();
            
            Ranker[] array = currentLeaderboard.ToArray();

            // Give the rankers their placement numbers
            for(int i = 0; i < array.Length; i++)   array[i].rankerPlace = i+1;
            
            currentLeaderboard = array.ToList();
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        SaveLeaderboardFile();
    }
}
