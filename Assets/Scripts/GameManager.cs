using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float m_worldWidth = 22.0f;
    public float m_lavaHeight = -3.0f;

    public delegate void ProcessObject(GameObject obj);

    public enum State
    {
        MAIN_MENU,
        GAME_ON,
        GAME_OVER,
        STORE,
        STAGE_CLEAR,
    }
    static GameManager s_theManager;
    Bounds m_screenBounds;  // screen boundaries in world space
    State m_state = State.MAIN_MENU;
    bool m_isPaused = false;

    public static readonly string[] s_levels =
    {
        "Levels/Level01",
        "Levels/Level02",
        "Levels/Level03",
        "Levels/Level04",
    };

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
        SaveData save = SaveData.Get();
        save.ResetGame();
        ChangeState(State.GAME_OVER);
    }

    public void StageClear()
    {
        SaveData save = SaveData.Get();
        save.SetPlayerHP(Player.Get().NumEgg());
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
        return s_levels[save.GetCurrentLevel()];
    }

    IEnumerator GameOverCountDown()
    {
        GameUI.Get().GameOver();
        GameUI.Get().SetHint("Catch the eggs before they hit the lava!");

        // wait for all the eggs to hit the lava
        while (Egg.GetCount() > 0)
            yield return null;
        // and then wait 2 more seconds
        yield return new WaitForSecondsRealtime(2.0f);

        // return to main menu
        ChangeState(State.MAIN_MENU);
    }

    IEnumerator StageClearCountDown()
    {
        GameUI.Get().StageClear();
        GameUI.Get().SetHint("Catch the eggs before they hit the lava!");

        // wait 2 more seconds
        yield return new WaitForSecondsRealtime(2.0f);

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
        UpdateScreenBounds();
        SoundInstance.WarmUp();
        Egg.MakeEggPool();
        EnemyBird.WarmUp();
        {   // warm up the player
            GameObject playerObject = Resources.Load<GameObject>("Player");
            Player player = playerObject.GetComponent<Player>();
            player.WarmUp();
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
        EnemyBird.DoRepulse(Time.deltaTime);
    }

    void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                Egg.DeleteAll();
                EnemyBird.DeleteAll();
                Bullet.DeleteAll();
                SceneManager.LoadScene("MainMenu");
                break;
            case State.GAME_ON:
                SceneManager.LoadScene("Game");
                break;
            case State.STORE:
                SceneManager.LoadScene("Store");
                break;
            default:
                break;
        }
        SetState(newState);
    }

    void SetState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                MusicManager.Get().Play(MusicManager.SongType.TITLE);
                break;
            case State.GAME_ON:
                MusicManager.Get().Play(MusicManager.SongType.GAME);
                break;
            case State.GAME_OVER:
                StartCoroutine(GameOverCountDown());
                break;
            case State.STORE:
                MusicManager.Get().Play(MusicManager.SongType.SHOP);
                break;
            case State.STAGE_CLEAR:
                StartCoroutine(StageClearCountDown());
                break;
        }
        m_state = newState;
    }

    public void SetPause(bool set)
    {
        if (set)
        {
            Time.timeScale = 0.0f;
            m_isPaused = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            m_isPaused = false;
        }
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
}
