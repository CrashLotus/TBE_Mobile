using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public TextMeshProUGUI m_text;

    public void OnResumeButton()
    {
        GameManager.Get().SetPause(false);
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }
}
