using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float m_worldWidth = 12.0f;
    public float m_lavaHeight = -3.0f;

    public delegate void ProcessObject(GameObject obj);

    public enum State
    {
        MAIN_MENU,
        GAME_ON,
        GAME_OVER,
        STORE,
        STAGE_CLEAR,
        CREDITS,
    }
    public enum GameOverAdResult
    { 
        WAITING,
        SUCCESS,
        FAIL
    }
    static GameManager s_theManager;
    Bounds m_screenBounds;  // screen boundaries in world space
    State m_state = State.MAIN_MENU;
    static GameOverAdResult s_adResult = GameOverAdResult.WAITING;
    bool m_isPaused = false;

    public static readonly string[] s_levels =
    {
//        "Levels/TestWorm",
        "Levels/Level01",
        "Levels/Level02",
        "Levels/Level03",
        "Levels/Level04",
        "Levels/Level05",
        "Levels/Level06",
        "Levels/Level07",
        "Levels/Level08",
        "Levels/Level09",
        "Levels/Level10",
        "Levels/Level11",
        "Levels/Level12",
        "Levels/Level13",
        "Levels/Level14",
        "Levels/Level15",
        "Levels/Level16",
        "Levels/Level17",
        "Levels/Level18",
        "Levels/Level19",
        "Levels/Level20",
        "Levels/Level21",
        "Levels/Level22",
        "Levels/Level23",
        "Levels/KillLevel",
    };

    public class Hint
    {
        public class Entry
        {
            public string m_text;
            public int m_minLevel;
            public int m_maxLevel;
            public float m_weight;
        }

        public List<Entry> m_theList;
    }
    Hint m_hints;
    string m_hintText;
    const float s_stageClearWaitTime = 3.0f;

    public static GameManager Get()
    {
        if (null == s_theManager)
        {   // create it
            GameObject obj = new GameObject("GameManager");
            s_theManager = obj.AddComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }
        return s_theManager;
    }

    public Bounds GetScreenBounds()
    {
        return m_screenBounds;
    }

    public float GetLavaHeight()
    {
        return m_lavaHeight;
    }

    public State GetState()
    {
        return m_state;
    }

    public void OnNewGame()
    {
        SaveData.Get().ResetGame();
        ChangeState(State.GAME_ON);
    }

    public void GoToStore()
    {
        ChangeState(State.STORE);
    }

    public void GameOver()
    {
        ChangeState(State.GAME_OVER);
    }

    public void StageClear()
    {
        SaveData save = SaveData.Get();
        save.SetScore(Player.GetScore());
        save.SetPlayerHP(Player.NumEgg());
        int nextLevel = save.GetCurrentLevel();
        ++nextLevel;
        if (nextLevel >= s_levels.Length)
            nextLevel = s_levels.Length - 1;
        save.SetCurrentLevel(nextLevel);
        ChangeState(State.STAGE_CLEAR);
    }

    public static string GetCurrentLevelName()
    {
        SaveData save = SaveData.Get();
        int level = save.GetCurrentLevel();
        level = Mathf.Min(level, s_levels.Length - 1);
        return s_levels[level];
    }

    public static string GetCurrentScene()
    {
        SaveData save = SaveData.Get();
        int level = save.GetCurrentLevel();
        if (level > 19)
            return "Game_UFO";
        if (level > 14)
            return "Game_Dino";
        if (level > 9)
            return "Game_Paris";
        if (level > 5)
            return "Game_Tunguska";
        return "Game_NY";
    }

    public static int GetNumLevels()
    {
        return s_levels.Length;
    }

    IEnumerator GameOverCountDown()
    {
        SaveData save = SaveData.Get();
        s_adResult = GameOverAdResult.WAITING;
        bool showAd = save.GetCurrentLevel() > 0;
        GameUI.Get().GameOver(showAd);
        NextHint();
        GameUI.Get().SetHint(m_hintText);

        // wait for the user to watch the ad - or not
        if (showAd)
        {
            while (s_adResult == GameOverAdResult.WAITING)
                yield return null;
            if (s_adResult == GameOverAdResult.FAIL)
            {
                LeaderBoard.Get().AddScore(save.GetScore());
                save.ResetGame();
                ChangeState(State.MAIN_MENU);
                yield break;
            }
            ChangeState(State.GAME_ON);
            yield break;
        }

        // wait for all the eggs to hit the lava
        while (Egg.GetCount() > 0)
            yield return null;

        // and then wait 2 more seconds
        yield return new WaitForSecondsRealtime(2.0f);

        // return to main menu
        LeaderBoard.Get().AddScore(save.GetScore());
        save.ResetGame();
        ChangeState(State.MAIN_MENU);
    }

    static void OnGameOverAdCompleted(bool success)
    {
        s_adResult = success ? GameOverAdResult.SUCCESS : GameOverAdResult.FAIL;
    }

    public void ShowGameOverAd(bool show)
    {
        if (show)
            PurchaseManager.Get().ShowAd(OnGameOverAdCompleted);
        else
            OnGameOverAdCompleted(false);
    }

    IEnumerator StageClearCountDown()
    {
        GameUI.Get().StageClear();
        NextHint();
        GameUI.Get().SetHint(m_hintText);

        // wait 2 more seconds
        yield return new WaitForSecondsRealtime(s_stageClearWaitTime);

        // return to main menu
        OnContinueGame();
        ChangeState(State.STORE);
    }

    void UpdateScreenBounds()
    {
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        topRight.z = 100.0f;
        Vector3 botLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        botLeft.z = -100.0f;
        m_screenBounds = new Bounds(0.5f * (topRight + botLeft), topRight - botLeft);
    }

    void Awake()
    {
        if (null == s_theManager)
        {
            s_theManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // Boot Strap
        PurchaseManager.Get();
        LeaderBoard.Get();
        BulletTime.Get();
        UpdateScreenBounds();
        SoundInstance.WarmUp();
        Egg.MakeEggPool();
        TimeCrystal.MakeCrystalPool();
        EnemyBird.WarmUp();
        Meteor.MakeMeteorPool();
        Worm.WarmUp();
        {   // warm up the player
            GameObject playerObject = Resources.Load<GameObject>("Player");
            Player player = playerObject.GetComponent<Player>();
            player.WarmUp();
        }
        {   // load the hints
            TextAsset textAsset = Resources.Load<TextAsset>("Hints");
            StringReader textRead = new StringReader(textAsset.text);
            XmlSerializer xml = new XmlSerializer(typeof(Hint));
            m_hints = xml.Deserialize(textRead) as Hint;
            textRead.Close();
            Resources.UnloadAsset(textAsset);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SetState(State.GAME_ON);
        }
        else
        {
            SetState(State.MAIN_MENU);
        }
    }

    void Update()
    {
        UpdateScreenBounds();
        EnemyBird.DoRepulse(BulletTime.Get().GetDeltaTime(false));
    }

    void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                ClearGame();
                SceneManager.LoadScene("MainMenu");
                break;
            case State.GAME_ON:
                ClearGame();
                SceneManager.LoadScene(GetCurrentScene());
                break;
            case State.STORE:
                ClearGame();
                SceneManager.LoadScene("Store");
                break;
            case State.CREDITS:
                ClearGame();
                SceneManager.LoadScene("Credits");
                break;
            default:
                break;
        }
        SetState(newState);
    }

    void ClearGame()
    {
        Level level = Level.Get();
        if (null != level)
            level.Abort();
        Egg.DeleteAll();
        EnemyBird.DeleteAll();
        Worm.DeleteAll();
        Bullet.DeleteAll();
        Meteor.DeleteAll();
    }

    void SetState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                MusicManager.Get().Play(MusicManager.SongType.TITLE);
                break;
            case State.GAME_ON:
                AnalyticsManager.Get().LevelStart(SaveData.Get().GetCurrentLevel());
                MusicManager.Get().Play(MusicManager.SongType.GAME);
                break;
            case State.GAME_OVER:
                AnalyticsManager.Get().GameOver(SaveData.Get().GetCurrentLevel());
                StartCoroutine(GameOverCountDown());
                break;
            case State.STORE:
                MusicManager.Get().Play(MusicManager.SongType.SHOP);
                break;
            case State.STAGE_CLEAR:
                AnalyticsManager.Get().LevelComplete(SaveData.Get().GetCurrentLevel());
                StartCoroutine(StageClearCountDown());
                break;
            case State.CREDITS:
                MusicManager.Get().Play(MusicManager.SongType.CREDITS);
                break;
        }
        m_state = newState;
    }

    public void SetPause(bool set)
    {
        if (set)
        {
            m_isPaused = true;
        }
        else
        {
            m_isPaused = false;
        }
    }

    public bool GetPause()
    {
        return m_isPaused;
    }

    public void ReturnToMainMenu()
    {
        SetPause(false);
        ChangeState(State.MAIN_MENU);
    }

    public void OnContinueGame()
    {
        ChangeState(State.GAME_ON);
    }

    public void GoToCredits()
    {
        ChangeState(State.CREDITS);
    }

    void NextHint()
    {
        m_hintText = null;
        int levelIndex = SaveData.Get().GetCurrentLevel();
        List<Hint.Entry> hints = new List<Hint.Entry>();
        float totalWeight = 0.0f;
        foreach (Hint.Entry entry in m_hints.m_theList)
        {
            if (entry.m_minLevel <= levelIndex + 1)
            {
                if ((entry.m_maxLevel <= 0)
                    || (entry.m_maxLevel >= levelIndex + 1)
                    )
                {
                    hints.Add(entry);
                    totalWeight += entry.m_weight;
                }
            }
        }
        if (hints.Count > 0)
        {
            float rand = Random.Range(0.0f, totalWeight);
            foreach (Hint.Entry entry in hints)
            {
                if (rand <= entry.m_weight)
                {
                    m_hintText = entry.m_text;
                    break;
                }
                rand -= entry.m_weight;
            }
        }
    }
}
