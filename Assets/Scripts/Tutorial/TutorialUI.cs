//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

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
