using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class LeaderBoard : MonoBehaviour
{
    static LeaderBoard s_theLeaderBoard;

    enum State
    {
        INIT,
        READY,
    }
    State m_state = State.INIT;

    const string s_allTime = "ALL_TIME";
    LeaderboardScoresPage m_allTimeScores;

    public static LeaderBoard Get()
    {
        if (null == s_theLeaderBoard)
        {
            GameManager gm = GameManager.Get();
            s_theLeaderBoard = gm.gameObject.AddComponent<LeaderBoard>();
        }
        return s_theLeaderBoard;
    }

    private void Awake()
    {
        StartCoroutine(UpdateCo());
    }

    IEnumerator UpdateCo()
    {
        m_state = State.INIT;

        // wait for sign-in
        PurchaseManager pm = PurchaseManager.Get();
        while (false == pm.IsAuthenticationReady())
            yield return null;

        // wait for scores
        var task = LeaderboardsService.Instance.GetScoresAsync(s_allTime);
        yield return new WaitUntil(() => task.IsCompleted);
        m_allTimeScores = task.Result;

        Debug.Log("Leaderboard Ready");
        m_state = State.READY;
    }

    public async void AddScore(int score)
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(s_allTime, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
}
