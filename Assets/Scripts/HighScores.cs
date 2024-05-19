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
    public float m_startY = -278.0f;
    public float m_lineSpacing = 132.0f;
    public int m_maxDisplay = 10;
    public Sound m_menuSelect;
    public Sound m_typeSound;
    public Sound m_openKeyboardSound;
    public Sound m_closeKeyboardSound;
    string m_playerName;
    TextMeshProUGUI m_playerText;

#if USE_OSK
    Process m_keyboard;
#else
    TouchScreenKeyboard m_keyboard;
#endif

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScores());
    }

    IEnumerator LoadScores()
    {
        LeaderBoard lb = LeaderBoard.Get();
        while (false == lb.IsReady())
            yield return null;
        string yourId = PurchaseManager.Get().PlayerId();
        var scores = lb.GetAllTimeScores();
        int rank = 1;
        Vector3 pos = Vector3.zero;
        pos.y = m_startY;
        bool foundYours = false;
        foreach (var score in scores.Results)
        {
            foundYours |= DrawLine(score, pos, yourId);
            rank++;
            pos.y -= m_lineSpacing;
            if (rank > m_maxDisplay)
                break;
        }
    }

    bool DrawLine(LeaderboardEntry score, Vector3 pos, string yourId)
    {
        bool foundYours = false;
        GameObject line = Instantiate(m_linePrefab, transform);
        RectTransform rect = line.transform as RectTransform;
        rect.anchoredPosition = pos;
        {
            TextMeshProUGUI textMesh = line.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.text = score.Rank.ToString();
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

    public void OnExit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
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
