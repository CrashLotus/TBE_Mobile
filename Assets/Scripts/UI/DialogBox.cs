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

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    public delegate void OnDialogButton();
    OnDialogButton m_yes;
    OnDialogButton m_no;

    public void ShowDialog(string text, OnDialogButton onYes, OnDialogButton onNo)
    {
        gameObject.SetActive(true);
        m_text.text = text;
        m_yes = onYes;
        m_no = onNo;
    }

    public void OnDialogYes()
    {
        Debug.Log("OnDialogYes()");
        if (null != m_yes)
            m_yes();
        gameObject.SetActive(false);
    }

    public void OnDialogNo()
    {
        Debug.Log("OnDialogNo()");
        if (null != m_no)
            m_no();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UI_Utility.SelectUI(transform.Find("Yes").gameObject);
    }
}
