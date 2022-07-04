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
        }
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        m_optionsMenu.SetActive(!m_optionsMenu.activeSelf);
        m_menuSelect.Play();
    }

    public void OnCloseOptions()
    {
        m_menuSelect.Play();
    }

    public void OnQuit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }
}
