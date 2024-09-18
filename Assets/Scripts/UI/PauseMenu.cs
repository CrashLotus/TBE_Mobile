//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    public Sound m_menuSelect;

    public void OnPauseGame()
    {
        if (gameObject.activeSelf)
        {   // unpause
            GameManager.Get().SetPause(false);
            gameObject.SetActive(false);
        }
        else
        {   // pause
            GameManager.Get().SetPause(true);
            gameObject.SetActive(true);
            UI_Utility.SelectUI(m_pauseMenu.transform.Find("Resume").gameObject);
        }
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        bool openOptions = !m_optionsMenu.activeSelf;
        m_optionsMenu.SetActive(openOptions);
        m_menuSelect.Play();
        if (openOptions)
            UI_Utility.SelectUI(m_optionsMenu.transform.GetChild(0).Find("Close").gameObject);
        else
            UI_Utility.SelectUI(m_pauseMenu.transform.Find("Resume").gameObject);
    }

    public void OnCloseOptions()
    {
        m_menuSelect.Play();
        UI_Utility.SelectUI(m_pauseMenu.transform.Find("Resume").gameObject);
    }

    public void OnQuit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }
}
