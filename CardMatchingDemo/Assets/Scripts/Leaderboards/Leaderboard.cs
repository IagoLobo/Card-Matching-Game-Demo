using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Leaderboard
{
    public List<Ranker> leaderboardRankerList;

    public Leaderboard(List<Ranker> rankerList)
    {
        leaderboardRankerList = rankerList;
    }
}
