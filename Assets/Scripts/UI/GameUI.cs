using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject m_gameOver;
    public GameObject m_labelText;
    public GameObject m_hintText;

    static GameUI s_theUI;

    // Start is called before the first frame update
    void Start()
    {
        s_theUI = this;
        if (null != m_gameOver)
            m_gameOver.SetActive(false);
        if (null != m_labelText)
            m_labelText.SetActive(false);
        if (null != m_hintText)
            m_hintText.SetActive(false);
    }

    public static GameUI Get()
    {
        return s_theUI;
    }

    public void GameOver()
    {
        if (null != m_gameOver)
        {
            m_gameOver.SetActive(true);
            Animator anim = m_gameOver.GetComponent<Animator>();
            if (null != anim)
                anim.Play("GameOver");
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
                    anim.Play("Label");
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