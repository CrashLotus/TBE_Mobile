//#define USE_OSK

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
#if USE_OSK
using Process = System.Diagnostics.Process;
#endif

public class HighScores : MonoBehaviour
{
    public GameObject m_linePrefab;
    public GameObject m_allTimePanel;
    public GameObject m_weeklyPanel;
    public GameObject m_rightArrow;
    public GameObject m_leftArrow;
    public float m_startY = -278.0f;
    public float m_lineSpacing = 132.0f;
    public int m_maxDisplay = 10;
    public float m_timePerLine = 0.25f;
    public Sound m_menuSelect;
    public Sound m_typeSound;
    public Sound m_openKeyboardSound;
    public Sound m_closeKeyboardSound;
    public Sound m_swipeSound;
    public float m_slideSpeed = 1.0f;

    LeaderBoard.Board m_board = LeaderBoard.Board.ALL_TIME;
    string m_playerName;
    TextMeshProUGUI m_playerText;
    RectTransform m_rect;
    RectTransform m_canvas;
    List<GameObject> m_allLines = new List<GameObject>();

#if USE_OSK
    Process m_keyboard;
#else
    TouchScreenKeyboard m_keyboard;
#endif

    // Start is called before the first frame update
    void Start()
    {
        m_rect = GetComponent<RectTransform>();
        EnableArrows(false);
        StartCoroutine(LoadScores());
        Transform parent = transform.parent;
        while (parent)
        {
            Canvas canvas = parent.GetComponent<Canvas>();
            if (null != canvas)
            {
                m_canvas = canvas.GetComponent<RectTransform>();
                break;
            }
            parent = parent.parent;
        }
    }

    void Update()
    {
        float targetOffset = 0.0f;
        if (m_board == LeaderBoard.Board.WEEKLY)
            targetOffset = -m_canvas.rect.width;
        Vector2 pos = m_rect.anchoredPosition;
        float diff = targetOffset - pos.x;
        if (diff != 0.0f)
        {
            float maxSpeed = m_canvas.rect.width * m_slideSpeed * Time.deltaTime;
            bool done = false;
            if (diff < -maxSpeed)
                diff = -maxSpeed;
            else if (diff > maxSpeed)
                diff = maxSpeed;
            else
                done = true;
            pos.x += diff;
            m_rect.anchoredPosition = pos;
            if (done)
            {
                foreach (GameObject obj in m_allLines)
                    Destroy(obj);
                m_allLines.Clear();
                StartCoroutine(LoadScores());
            }
        }
    }

    IEnumerator LoadScores()
    {
        EnableArrows(false);
        LeaderBoard lb = LeaderBoard.Get();
        while (false == lb.IsReady())
            yield return null;
        string yourId = PurchaseManager.Get().PlayerId();
//        yourId = "playerId14";  // testing
        LeaderboardScoresPage scores = lb.GetScores(m_board);
        if (scores != null)
        {
            int yourIndex = -1;
            for (int i = 0; i < scores.Results.Count; ++i)
            {
                if (scores.Results[i].PlayerId == yourId)
                {
                    yourIndex = i;
                    break;
                }
            }
            Vector3 pos = Vector3.zero;
            pos.y = m_startY;
            int to = Mathf.Min(scores.Results.Count, m_maxDisplay);
            if (yourIndex >= 10)
                to = 8;
            Transform panel = m_allTimePanel.transform;
            if (m_board == LeaderBoard.Board.WEEKLY)
                panel = m_weeklyPanel.transform;
            for (int i = 0; i < to; ++i)
            {
                var score = scores.Results[i];
                DrawLine(score, pos, yourId, panel);
                pos.y -= m_lineSpacing;
                yield return new WaitForSecondsRealtime(m_timePerLine);
            }
            if (yourIndex >= 0 && yourIndex >= 10)
            {
                yield return new WaitForSecondsRealtime(m_timePerLine); // a double-delay before the player's line
                pos.y -= 0.5f * m_lineSpacing;
                var score = scores.Results[yourIndex];
                DrawLine(score, pos, yourId, panel);
            }
        }
        EnableArrows(true);
    }

    bool DrawLine(LeaderboardEntry score, Vector3 pos, string yourId, Transform panel)
    {
        bool foundYours = false;
        GameObject line = Instantiate(m_linePrefab, panel);
        m_allLines.Add(line);
        RectTransform rect = line.transform as RectTransform;
        rect.anchoredPosition = pos;
        {
            int rank = score.Rank + 1;
            TextMeshProUGUI textMesh = line.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.text = rank.ToString();
        }
        {
            TextMeshProUGUI textMesh = line.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            textMesh.text = ((int)score.Score).ToString();
        }
        {
            TextMeshProUGUI textMesh = line.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            textMesh.text = score.PlayerName;
            Button button = line.transform.GetChild(2).GetComponent<Button>();
            if (button)
            {
                if (score.PlayerId == yourId)
                {
                    button.onClick.AddListener(OnClickName);
                    button.enabled = true;
                }
                else
                {
                    button.enabled = false;
                }
            }
            if (score.PlayerId == yourId)
            {
                m_playerText = textMesh;
            }
        }
        if (score.PlayerId == yourId)
        {
            foundYours = true;
            Animator anim = line.GetComponent<Animator>();
            if (null != anim)
                anim.Play("You");
            m_playerName = score.PlayerName;
        }
        return foundYours;
    }

    void EnableArrows(bool enable)
    {
        m_leftArrow.SetActive(enable);
        m_rightArrow.SetActive(enable);
    }

    public void OnExit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }

    public void OnRight()
    {
        if (null != m_swipeSound)
            m_swipeSound.Play();
        m_board = LeaderBoard.Board.WEEKLY;
    }

    public void OnLeft()
    {
        if (null != m_swipeSound)
            m_swipeSound.Play();
        m_board = LeaderBoard.Board.ALL_TIME;
    }

    public void OnClickName()
    {
        Debug.Log("OnClickName");
#if USE_OSK
        if (null == m_keyboard)
        {
            if (null != m_openKeyboardSound)
                m_openKeyboardSound.Play();
            m_keyboard = Process.Start("osk.exe");
            StartCoroutine(UpdateOSK());
        }
#else
        if (null == m_keyboard)
        {
            if (null != m_openKeyboardSound)
                m_openKeyboardSound.Play();
            m_keyboard = TouchScreenKeyboard.Open(m_playerName);
            StartCoroutine(UpdateKeyboard());
        }
#endif
    }

#if USE_OSK
    IEnumerator UpdateOSK()
    {
        yield return new WaitForSeconds(2.0f);
        while (null != m_keyboard)
        {
            if (m_keyboard.HasExited)
            {
                m_keyboard = null;
                break;
            }
            else
            {
                Debug.Log(System.Console.ReadLine());
            }
            yield return null;
        }
    }
#else
    IEnumerator UpdateKeyboard()
    {
        if (m_keyboard == null)
            yield break;
        while (false == m_keyboard.active)
            yield return null;
        while (m_keyboard.active)
        {
            string name = m_keyboard.text;
            if (name != m_playerName)
            {
                m_playerName = name;
                if (m_playerText != null)
                    m_playerText.text = m_playerName;
                if (null != m_typeSound)
                    m_typeSound.Play();
            }
            yield return null;
        }
        m_keyboard = null;
        Debug.Log("Closed with name = " + m_playerName);
        LeaderBoard.Get().SetPlayerName(m_playerName);
        if (null != m_closeKeyboardSound)
            m_closeKeyboardSound.Play();
    }
#endif
}
