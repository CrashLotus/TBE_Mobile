using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject m_gameOver;
    public GameObject m_stageClear;
    public GameObject m_labelText;
    public GameObject m_stageText;
    public GameObject m_hintText;
    public Joystick m_fixedJoystick;
    public Joystick m_floatJoystick;
    public GameObject m_fireButtonSingle;
    public GameObject m_fireButtonSplit;

    Joystick m_joystick;

    static GameUI s_theUI;

    // Start is called before the first frame update
    void Start()
    {
        s_theUI = this;
        if (null != m_gameOver)
            m_gameOver.SetActive(false);
        if (null != m_stageClear)
            m_stageClear.SetActive(false);
        if (null != m_labelText)
            m_labelText.SetActive(false);
        if (null != m_hintText)
            m_hintText.SetActive(false);
        if (null != m_stageText)
        {
            TextMeshProUGUI text = m_stageText.GetComponent<TextMeshProUGUI>();
            if (null != text)
            {
                text.text = "Stage " + (SaveData.Get().GetCurrentLevel() + 1);
                Animator anim = m_stageText.GetComponent<Animator>();
                if (null != anim)
                    anim.Play("Stage", -1, 0.0f);
            }
        }
        UpdateOptions();
    }

    public static GameUI Get()
    {
        return s_theUI;
    }

    public void UpdateOptions()
    {
        switch (PlayerPrefs.GetInt("joystick", 0))
        {
            case 0: // fixed
            default:
                m_fixedJoystick.gameObject.SetActive(true);
                m_floatJoystick.gameObject.SetActive(false);
                m_joystick = m_fixedJoystick;
                break;
            case 1: // float
                m_fixedJoystick.gameObject.SetActive(false);
                m_floatJoystick.gameObject.SetActive(true);
                m_joystick = m_floatJoystick;
                break;
        }

        SaveData data = SaveData.Get();
        if (data.HasUpgrade("AIM"))
        {
            m_fireButtonSingle.SetActive(false);
            m_fireButtonSplit.SetActive(true);
        }
        else
        {
            m_fireButtonSingle.SetActive(true);
            m_fireButtonSplit.SetActive(false);
        }
    }

    public Joystick GetJoystick()
    {
        return m_joystick;
    }

    public void GameOver(bool showAd)
    {
        if (null != m_gameOver)
        {
            m_gameOver.SetActive(true);
            Animator anim = m_gameOver.GetComponent<Animator>();
            if (null != anim)
            {
                if (showAd)
                    anim.Play("GameOverAd", -1, 0.0f);
                else
                    anim.Play("GameOver", -1, 0.0f);
            }
        }
    }

    public void OnShowAd(bool show)
    {
        GameManager.Get().ShowGameOverAd(show);
    }

    public void StageClear()
    {
        if (null != m_stageClear)
        {
            m_stageClear.SetActive(true);
            Animator anim = m_stageClear.GetComponent<Animator>();
            if (null != anim)
                anim.Play("GameOver");  // we're re-using the same animation here
        }
    }

    public void SetLabel(string label)
    { 
        if (null != m_labelText)
        {
            m_labelText.SetActive(true);
            TextMeshProUGUI text = m_labelText.GetComponent<TextMeshProUGUI>();
            if (null != text)
            {
                text.text = label;
                Animator anim = m_labelText.GetComponent<Animator>();
                if (null != anim)
                    anim.Play("Label", -1, 0.0f);
            }
        }
    }

    public void SetHint(string hint)
    {
        if (null != m_hintText)
        {
            m_hintText.SetActive(true);
            TextMeshProUGUI text = m_hintText.GetComponent<TextMeshProUGUI>();
            if (null != text)
            {
                text.text = hint;
                Animator anim = m_hintText.GetComponent<Animator>();
                if (null != anim)
                    anim.Play("Hint");
            }
        }
    }
}
