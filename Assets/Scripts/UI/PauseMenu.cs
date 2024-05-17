using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            EventSystem.current.SetSelectedGameObject(m_pauseMenu.transform.Find("Resume").gameObject);
        }
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        bool openOptions = !m_optionsMenu.activeSelf;
        m_optionsMenu.SetActive(openOptions);
        m_menuSelect.Play();
        if (openOptions)
            EventSystem.current.SetSelectedGameObject(m_optionsMenu.transform.GetChild(0).Find("Close").gameObject);
        else
            EventSystem.current.SetSelectedGameObject(m_pauseMenu.transform.Find("Resume").gameObject);
    }

    public void OnCloseOptions()
    {
        m_menuSelect.Play();
        EventSystem.current.SetSelectedGameObject(m_pauseMenu.transform.Find("Resume").gameObject);
    }

    public void OnQuit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }
}
