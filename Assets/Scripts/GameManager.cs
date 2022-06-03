using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float m_worldWidth = 22.0f;
    public float m_lavaHeight = -3.0f;

    public enum State
    {
        MAIN_MENU,
        GAME_ON,
        GAME_OVER
    }
    static GameManager s_theManager;
    Bounds m_screenBounds;  // screen boundaries in world space
    State m_state = State.MAIN_MENU;

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
        m_state = State.GAME_ON;
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        m_state = State.GAME_OVER;
        StartCoroutine(GameOverCountDown());
    }

    IEnumerator GameOverCountDown()
    {
        // wait for all the eggs to hit the lava
        while (Egg.GetCount() > 0)
            yield return null;
        // and then wait 2 more seconds
        yield return new WaitForSecondsRealtime(2.0f);

        // return to main menu
        EnemyBird.DeleteAll();
        m_state = State.MAIN_MENU;
        SceneManager.LoadScene("MainMenu");
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
        }
        UpdateScreenBounds();
    }

    void Update()
    {
        UpdateScreenBounds();
        EnemyBird.DoRepulse(Time.deltaTime);
    }
}
