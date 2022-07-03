using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    public Sound m_menuSelect;

    public enum State
    {
        PAUSED,
        OPTIONS
    }
    State m_state = State.PAUSED;

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
            SetState(State.PAUSED);
        }
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        SetState(State.OPTIONS);
        m_optionsMenu.SetActive(true);
        m_menuSelect.Play();
    }

    public void OnCloseOptions()
    {
        SetState(State.PAUSED);
        m_menuSelect.Play();
    }

    public void OnQuit()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }

    void SetState(State newState)
    {
        switch (newState)
        {
            case State.PAUSED:
                m_optionsMenu.SetActive(false);
                m_pauseMenu.SetActive(true);
                break;
            case State.OPTIONS:
                m_optionsMenu.SetActive(true);
                m_pauseMenu.SetActive(false);
                break;
        }
    }
}
