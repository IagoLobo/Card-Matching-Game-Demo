using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIManager : MonoBehaviour
{
    public GameObject rankerHeadline;
    public VerticalLayoutGroup rankerLayout;
    public GameObject NoLeaderboardsMessage;
    public GameObject rankerPrefab;
    private List<Ranker> rankers;

    public void ShowLeaderboards()
    {
        // See if Leaderboard is empty or not
        if(LeaderboardManager.Instance.LoadLeaderboardFile().Count < 1 || LeaderboardManager.Instance.LoadLeaderboardFile() == null)
        {
            NoLeaderboardsMessage.SetActive(true);
            rankerHeadline.SetActive(false);
            rankerLayout.gameObject.SetActive(false);
        }
        else
        {
            NoLeaderboardsMessage.SetActive(false);
            rankerHeadline.SetActive(true);
            rankerLayout.gameObject.SetActive(true);

            ClearLeaderboards();
            FillLeaderboards(LeaderboardManager.Instance.currentLeaderboard);
        }
    }

    public void FillLeaderboards(List<Ranker> rankers)
    {
        foreach(Ranker r in rankers)
        {
            GameObject ranker = Instantiate(rankerPrefab, rankerLayout.transform.position, Quaternion.identity);
            ranker.transform.SetParent(rankerLayout.transform, false);
            ranker.transform.GetChild(0).GetComponent<Text>().text = r.rankerPlace.ToString("00");
            ranker.transform.GetChild(1).GetComponent<Text>().text = r.rankerName;
            ranker.transform.GetChild(2).GetComponent<Text>().text = r.rankerFinalScore.ToString();
            ranker.transform.GetChild(3).GetComponent<Text>().text = TimeElapsedForRanker(r.rankerTimeElapsed);
        }
    }

    public void ClearLeaderboards()
    {
        if(rankerLayout.transform.childCount > 0)
        {
            List<GameObject> children = new List<GameObject>();

            for(int i = 0; i < rankerLayout.transform.childCount; i++)
                children.Add(rankerLayout.transform.GetChild(i).gameObject);

            foreach(GameObject obj in children) Destroy(obj);
        }
    }

    public string TimeElapsedForRanker(float te)
    {
        // Show time elapsed in minutes & seconds
        int minutes = (int) (te / 60);
        int seconds = (int) (te - minutes * 60);
        return (minutes.ToString() + ':' + seconds.ToString("00"));
    }
}
