//#define TEST_LEADERS

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

    public bool IsReady()
    {
        return m_state == State.READY;
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
        m_state = State.INIT;
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(s_allTime, score);
        m_allTimeScores = await LeaderboardsService.Instance.GetScoresAsync(s_allTime);
        m_state = State.READY;
    }

    public LeaderboardScoresPage GetAllTimeScores()
    {
        if (m_state != State.READY)
            return null;

#if TEST_LEADERS
        List<LeaderboardEntry> list = new List<LeaderboardEntry>();
        list.Add(new LeaderboardEntry("playerId1",  "Player1",   1,  999999.0));
        list.Add(new LeaderboardEntry("playerId2",  "Player2",   2,  900000.0));
        list.Add(new LeaderboardEntry("playerId3",  "Player3",   3,  800000.0));
        list.Add(new LeaderboardEntry("playerId4",  "Player4",   4,  700000.0));
        list.Add(new LeaderboardEntry("playerId5",  "Player5",   5,  600000.0));
        list.Add(new LeaderboardEntry("playerId6",  "Player6",   6,  500000.0));
        list.Add(new LeaderboardEntry("playerId7",  "Player7",   7,  400000.0));
        list.Add(new LeaderboardEntry("playerId8",  "Player8",   8,  300000.0));
        list.Add(new LeaderboardEntry("playerId9",  "Player9",   9,  200000.0));
        list.Add(new LeaderboardEntry("playerId10", "Player10",  10, 100000.0));
        list.Add(new LeaderboardEntry("playerId11", "Player11",  11, 10000.0));
        list.Add(new LeaderboardEntry("playerId12", "Player12",  12, 1000.0));
        list.Add(new LeaderboardEntry("playerId13", "Player13",  13, 900.0));
        list.Add(new LeaderboardEntry("playerId14", "Player14",  14, 800.0));
        list.Add(new LeaderboardEntry("playerId15", "Player15",  15, 600.0));
        LeaderboardScoresPage testPage = new LeaderboardScoresPage(0, 0, 1, list);
        return testPage;
#endif
        return m_allTimeScores;
    }

    public void SetPlayerName(string name)
    {
        AuthenticationService.Instance.UpdatePlayerNameAsync(name);
    }
}
