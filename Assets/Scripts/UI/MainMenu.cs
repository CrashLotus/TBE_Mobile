using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject m_optionsMenu;
    public Sound m_menuSelect;

    public void OnNewGame()
    {
        GameManager.Get().OnNewGame();
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        m_optionsMenu.SetActive(!m_optionsMenu.activeSelf);
        m_menuSelect.Play();
    }

    public void OnQuitGame()
    {
        Application.Quit();
        m_menuSelect.Play();
    }
}
