using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<int> randomCardSequence;
    public List<int> cardIDsRevealed;
    public string playerName;
    public int numberOfTries;
    public float timeElapsed;

    public GameState(string pn, int numTries, float te, List<int> secretSequence, List<int> idsRevealed)
    {
        randomCardSequence = secretSequence;
        cardIDsRevealed = idsRevealed;
        playerName = pn;
        numberOfTries = numTries;
        timeElapsed = te;
    }
}